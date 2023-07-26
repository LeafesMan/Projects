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

public static class AudioHandler
{
    [SerializeField] private static int audioPoolSize;
    private static Transform audioSources;


    //Lists of standard Audio sources
    private static List<AudioSource> freeAudioSources = new List<AudioSource>();
    private static List<(AudioSource source, float timeStamp)> busyAudioSources = new List<(AudioSource source, float timeStamp)>();

    //Lists of Positional AudioSources
    private static List<AudioSource> freePositionalAudioSources = new List<AudioSource>();
    private static List<(AudioSource source, float timeStamp)> busyPositionalAudioSources = new List<(AudioSource source, float timeStamp)>();



    /// <summary>
    /// Plays positional Audio
    /// </summary>
    public static void Play(AudioClip clip, float volume, float pitch, Vector2 position)
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
    public static void Play(AudioClip clip, float volume, float pitch)
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

    public static AudioSource GetAudioSource(List<AudioSource> freeAudioSources, List<(AudioSource source, float timeStamp)> busyAudioSources, bool isPositional)
    {   // Gets an audio source and places it in the used list
        // Uses free sources when possible
        // When there are no free sources creates a new one
        // OR   uses the oldest used source if the pool is full

        if(audioSources == null) audioSources = new GameObject("Audio Sources").transform; //Lazy Create AudioSources Object

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

    public static AudioSource CreateAudioSource() => audioSources.gameObject.AddComponent<AudioSource>();
    public static AudioSource CreatePositionalAudioSource()
    {   // Creates an audio source object
        AudioSource source = new GameObject("Positional Audio Source").AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.transform.SetParent(audioSources.transform);
        return source;
    }

    /// <summary>
    /// Moves all completed audio sources in the busy list to the free list
    /// </summary>
    public static void RefreshAudioSources(List<AudioSource> freeAudioSources, List<(AudioSource source, float timeStamp)> busyAudioSources)
    {   
        for (int i = busyAudioSources.Count - 1; i >= 0; i--)
            if (busyAudioSources[i].source.clip.length <= Time.time - busyAudioSources[i].timeStamp) // If busy audio source has finished
            {
                freeAudioSources.Add(busyAudioSources[i].source);
                busyAudioSources.RemoveAt(i);
            }
    }
}
