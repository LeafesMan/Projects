////////////////////////////////////
//
//  Author: Ian
//
//  Project: The Wall
//  
//  Date: 12/15/22
//       Audio handler script will listen for audio event calls
//       and play audio based on the VaribaleAudioClip passed in
//  Date: 1/6/23
//      Now uses audio source pooling & supports positional audio
// 
////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private int audioPoolSize;
    [SerializeField] private EventHandler e;

    private List<AudioSource> freeAudioSources = new List<AudioSource>();
    private List<(AudioSource source, float timeStamp)> busyAudioSources = new List<(AudioSource source, float timeStamp)>();
    private Transform audioSourcePool;

    //Subscribe and Unsubscribe
    private void OnEnable() => e.audioEvent += Play;
    private void OnDisable() => e.audioEvent -= Play;

    private void Start()
    {
        audioSourcePool = new GameObject("Audio Source Pool").transform;
        audioSourcePool.SetParent(transform);
    }
    public void Play(Audio audio, Vector2 position)
    {   //Get Clip and Audio source
        VariableAudioClip clip = audio.GetClip();
        AudioSource source = GetAudioSource();

        //Setup audio source
        source.transform.position = position;
        source.volume = clip.GetVolume();
        source.pitch = clip.GetPitch();
        source.clip = clip.clip;

        //Play sound
        source.Play();

        //Add source to used audio sources
        busyAudioSources.Add((source, Time.time));
    }

    public AudioSource GetAudioSource()
    {   // Gets an audio source and places it in the used list
        // Uses free sources when possible
        // When there are no free sources creates a new one
        // OR   uses the oldest used source if the pool is full
        RefreshAudioSources();
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
            source = CreateAudioSource();
        return source;
    }
    public AudioSource CreateAudioSource()
    {   // Creates an audio source object
        AudioSource source = new GameObject("Positional Audio Source").AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.transform.SetParent(audioSourcePool.transform);
        return source;
    }

    public void RefreshAudioSources()
    {   // Moves all completed audio sources in the busy list to the free list
        for (int i = busyAudioSources.Count - 1; i >= 0; i--)
            if (busyAudioSources[i].source.clip.length <= Time.time - busyAudioSources[i].timeStamp) // If busy audio source has finished
            {
                freeAudioSources.Add(busyAudioSources[i].source);
                busyAudioSources.RemoveAt(i);
            }
    }
}
