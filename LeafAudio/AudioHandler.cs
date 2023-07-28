/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/26/23 
 *  
 *  Desc: Static script for playing & pooling audio
 */

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private int audioPoolSize;

    //Lists of standard Audio sources
    private static List<AudioSource> freeAudioSources = new List<AudioSource>();
    private static List<(AudioSource source, float timeStamp)> busyAudioSources = new List<(AudioSource source, float timeStamp)>();

    //Lists of Positional AudioSources
    private static List<AudioSource> freePositionalAudioSources = new List<AudioSource>();
    private static List<(AudioSource source, float timeStamp)> busyPositionalAudioSources = new List<(AudioSource source, float timeStamp)>();

    //Fadeable Audio Sources
    //Each Fadeable audio source element has two audio sources for fading in a new clip
    private static List<(AudioSource, AudioSource)> fadeableAudioSourcePairs = new List<(AudioSource,AudioSource)>();


    //Static Events
    public static Action<AudioClip, float, float> PlayClipDelegate;
    public static Action<AudioClip, float, float, Vector2> PlayPositionalClipDelegate;
    public static Action<AudioClip, float, int> FadeClipDelegate;


    private void OnEnable()
    {
        PlayClipDelegate += Play;
        PlayPositionalClipDelegate+= Play;
        FadeClipDelegate += FadeIn;
    }
    private void OnDisable()
    {
        PlayClipDelegate -= Play;
        PlayPositionalClipDelegate -= Play;
        FadeClipDelegate -= FadeIn;
    }

    /// <summary>
    /// Plays positional Audio
    /// </summary>
    public void Play(AudioClip clip, float volume, float pitch, Vector2 position)
    {   //Get Clip and Audio source
        AudioSource source = GetAudioSource(freePositionalAudioSources, busyPositionalAudioSources, true);

        //Setup audio source
        source.transform.position = position;
        source.volume = volume;
        source.pitch = pitch;
        source.clip = clip;

        //Play sound
        source.Play();

        //Add source to used audio sources
        busyPositionalAudioSources.Add((source, Time.time));
    }
    /// <summary>
    /// Plays Audio
    /// </summary>
    public void Play(AudioClip clip, float volume, float pitch)
    {
        //Get Clip and Audio source
        AudioSource source = GetAudioSource(freeAudioSources, busyAudioSources, false);

        //Setup audio source
        source.volume = volume;
        source.pitch = pitch;
        source.clip = clip;

        //Play sound
        source.Play();

        //Add source to used audio sources
        busyAudioSources.Add((source, Time.time));
    }
    private AudioSource GetAudioSource(List<AudioSource> freeAudioSources, List<(AudioSource source, float timeStamp)> busyAudioSources, bool isPositional)
    {   // Gets an audio source and places it in the used list
        // Uses free sources when possible
        // When there are no free sources creates a new one
        // OR   uses the oldest used source if the pool is full

        RefreshAudioSources(freeAudioSources, busyAudioSources); //Frees up Audio Sources that are done

        AudioSource source;
        if (freeAudioSources.Count != 0) //If available use one
        {
            source = freeAudioSources[0];
            freeAudioSources.RemoveAt(0);
        }
        else if (freeAudioSources.Count == 0 && busyAudioSources.Count == audioPoolSize) //If none available and full
        {
            source = busyAudioSources[0].source;
            busyAudioSources.RemoveAt(0);
        }
        else //No source available and pool not full
            source = isPositional ? CreatePositionalAudioSource() : CreateAudioSource();
        return source;
    }
    private AudioSource CreateAudioSource() => gameObject.AddComponent<AudioSource>();
    private AudioSource CreatePositionalAudioSource()
    {   // Creates an audio source object
        AudioSource source = new GameObject("Positional Audio Source").AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.transform.SetParent(transform);
        return source;
    }
    /// <summary>
    /// Moves all completed audio sources in the busy list to the free list
    /// </summary>
    private void RefreshAudioSources(List<AudioSource> freeAudioSources, List<(AudioSource source, float timeStamp)> busyAudioSources)
    {   
        for (int i = busyAudioSources.Count - 1; i >= 0; i--)
            if (busyAudioSources[i].source.clip.length <= Time.time - busyAudioSources[i].timeStamp) // If busy audio source has finished
            {
                freeAudioSources.Add(busyAudioSources[i].source);
                busyAudioSources.RemoveAt(i);
            }
    }

    public void FadeIn(AudioClip clip, float fadeDuration, int fadeIndex)
    {   //If slot has not been created create new Pairs of fadeable audio sources until slot is created
        while (fadeableAudioSourcePairs.Count <= fadeIndex)
        {
            fadeableAudioSourcePairs.Add(new(gameObject.AddComponent<AudioSource>(), gameObject.AddComponent<AudioSource>()));
            fadeableAudioSourcePairs[fadeIndex].Item1.loop = true;
            fadeableAudioSourcePairs[fadeIndex].Item2.loop = true;
        }

        //Start fading out the faded in AudioSource
        StartCoroutine(FadeVolume(fadeableAudioSourcePairs[fadeIndex].Item2, 0, fadeDuration));

        //Fade in faded out Audio Source, replace it's clip with clip to fade in, and set volume to 0
        fadeableAudioSourcePairs[fadeIndex].Item2.clip = clip;
        fadeableAudioSourcePairs[fadeIndex].Item2.volume = 0;
        StartCoroutine(FadeVolume(fadeableAudioSourcePairs[fadeIndex].Item2, 1, fadeDuration));

        //Swap faded in AudioSource with the faded out AudioSource in the audioSourcePairs tuple
        fadeableAudioSourcePairs[fadeIndex] = new(fadeableAudioSourcePairs[fadeIndex].Item2, fadeableAudioSourcePairs[fadeIndex].Item1);
    }

    /// <summary>
    /// Fades volume from current value to targetVolume over duration.
    /// </summary>
    private IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float startTime = Time.time;

        //Lerp from start Volume to target Volume over duration
        while(Time.time - startTime <= duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, (Time.time - startTime) / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }
}
