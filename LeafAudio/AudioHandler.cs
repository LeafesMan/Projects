/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/26/23 
 *  
 *  Desc: Script for playing & pooling audio.
 *      Attach this component to one object in your scene to listen for and handle audio events.
 */

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

public class AudioHandler : MonoBehaviour
{
    [SerializeField, Tooltip("NOT IMPLEMENTED")] bool persist;
    [SerializeField, Tooltip("How many audio sources may be pooled.\nThis number has no bearing on looping audio sources.")] int poolSize;

    /// <summary>
    /// Audio Source Pool. Sorted in ascending order by End Time
    /// </summary>
    List<PooledAudioSource> pool = new();

    /// <summary>
    /// Each Looping audio source element has two audio sources for fading in a new looping clip
    /// </summary>
    Dictionary<uint, (AudioSource, AudioSource)> loopingPool = new();




    // Static Events
    public delegate void ClipHandler(ClipSpecs specs);
    public static ClipHandler Play;

    public delegate void PositionalClipHandler(ClipSpecs specs, Vector3 position);
    public static PositionalClipHandler PlayPositional;

    public delegate void ParentedClipHandler(ClipSpecs specs, Transform parent, Vector3 offset);
    public static ParentedClipHandler PlayParented;

    public delegate void LoopingClipHandler(ClipSpecs specs, float fadeInTime, uint slot);
    public static LoopingClipHandler PlayLooping;


    /// <summary>
    /// Struct for data stored about every source in the pool.
    /// </summary>
    class PooledAudioSource
    {
        readonly AudioSource source;
        public Transform parent;
        public Vector3 position;
        public float endTime;

        public PooledAudioSource(AudioSource source) => this.source = source;

        /// <summary>
        /// Setups a pooled audio source with a new set of parameters
        /// </summary>
        public void Setup(ClipSpecs specs, float spatialBlend, Transform parent, Vector3 position)
        {   // Audio Data
            source.clip   = specs.clip;
            source.volume = specs.volume;
            source.pitch  = specs.pitch;
            source.spatialBlend = spatialBlend;

            // Transform Data
            this.parent = parent;
            this.position = position;

            // End Time stamp based on clip length
            endTime = specs.clip.length + Time.time;
        }

        /// <summary>
        /// Plays the pooled audio source.
        /// </summary>
        public void Play() => source.Play();

        /// <summary>
        /// Whether the pooled audio source has finished its clip
        /// </summary>
        public bool IsAvailable => Time.time > endTime;
        public void UpdatePosition()
        {
            if (parent == null) return;

            // Set to parents position plus the offset
            // * Position acts as offset when parented
            source.transform.position = parent.position + position;
        }
    }



    private void OnEnable()
    {
        Play += PlayClip2D;
        PlayPositional += PlayClipPositional;
        PlayParented += PlayClipParented;
        PlayLooping += PlayClipLooping;
    }
    private void OnDisable()
    {
        Play -= PlayClip2D;
        PlayPositional -= PlayClipPositional;
        PlayParented -= PlayClipParented;
        PlayLooping -= PlayClipLooping;
    }
    /// <summary>
    /// Updates position of parented audio sources
    /// </summary>
    private void FixedUpdate()
    {
        foreach (PooledAudioSource pooledSource in pool)
            pooledSource.UpdatePosition();
    }

    /// <summary>
    /// Plays a Clip with the given parameters
    /// </summary>
    public void PlayClip(ClipSpecs specs, float spatialBlend, Transform parent, Vector3 pos)
    {   // Get Audio source
        PooledAudioSource pooledSource = GetAudioSource();

        // Setup audio source
        pooledSource.Setup(specs, spatialBlend, parent, pos);

        // Play Audio Source
        pooledSource.Play();

        // Add source to used audio sources
        Sort(pooledSource);
    }

    /// <summary>
    /// Plays 2D Audio
    /// </summary>
    public void PlayClip2D(ClipSpecs specs) => PlayClip(specs, 0, null, Vector3.zero);
    /// <summary>
    /// Plays positional Audio
    /// </summary>
    public void PlayClipPositional(ClipSpecs specs, Vector3 position) => PlayClip(specs, 1, null, position);
    /// <summary>
    /// Plays a positional audio source parented to an object
    /// </summary>
    public void PlayClipParented(ClipSpecs specs, Transform parent, Vector3 offset) => PlayClip(specs, 1, parent, offset);


    /// <summary>
    /// Plays Looping Audio
    /// </summary>
    public void PlayClipLooping(ClipSpecs specs, float fadeDuration, uint slot)
    {   // If Audio Source pair hasnt been created for this slot create it
        if (!loopingPool.ContainsKey(slot))
        {
            loopingPool.Add(slot, new (gameObject.AddComponent<AudioSource>(), gameObject.AddComponent<AudioSource>()));
            loopingPool[slot].Item1.loop = true;
            loopingPool[slot].Item2.loop = true;
        }

           

        //Start fading out the faded in AudioSource
        StartCoroutine(FadeVolume(loopingPool[slot].Item2, loopingPool[slot].Item2.volume, 0, fadeDuration));

        //Fade in faded out Audio Source, replace it's clip with clip to fade in, and set volume to 0
        loopingPool[slot].Item2.clip = specs.clip;
        loopingPool[slot].Item2.pitch = specs.pitch;
        StartCoroutine(FadeVolume(loopingPool[slot].Item2, 0, specs.volume, fadeDuration));

        //Swap faded in AudioSource with the faded out AudioSource in the audioSourcePairs tuple
        loopingPool[slot] = new(loopingPool[slot].Item2, loopingPool[slot].Item1);
    }

    private PooledAudioSource GetAudioSource()
    {   // Grabs a Pooled audio source to use for playing a sound
        // Uses free sources when possible
        // When there are no free sources creates a new one
        // OR   uses the oldest used source if the pool is full
        PooledAudioSource toReturn;


        // Pool has Available Source --> Return it
        if (pool.Count != 0 && pool[0].IsAvailable)
            toReturn = pool[0];
        // Pool Full --> Return Source that is closest to complete
        else if (pool.Count == poolSize)
            toReturn = pool[0];
        // Pool Not Full --> Create new Source
        else
        {   // Create and  reparent an audio source
            AudioSource audioSource = new GameObject("PooledAudioSource").AddComponent<AudioSource>();
            audioSource.transform.SetParent(transform);

            toReturn = new PooledAudioSource(audioSource);
        }

        return toReturn;
    }



    /// <summary>
    /// Fades volume from current value to targetVolume over duration.
    /// </summary>
    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        float startTime = Time.time;

        //Lerp from start Volume to target Volume over duration
        while(Time.time - startTime <= duration)
        {
            source.volume = Mathf.Lerp(from, to, (Time.time - startTime) / duration);
            yield return null;
        }

        source.volume = to;
    }


    /// <summary>
    /// Sorts the pool by ascending end time.<br></br>
    /// ***Assumes the PooledSource passed in is the only one that has changed
    /// </summary>
    private void Sort(PooledAudioSource toInsert)
    {
        pool.Remove(toInsert);

        // Find the
        int i = 0;
        for ( ; i < pool.Count; i++)
            if (pool[i].endTime > toInsert.endTime)
                return;

        pool.Insert(i, toInsert);
    }
}
