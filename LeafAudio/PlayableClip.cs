/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/26/23 
 *  
 *  Desc: Interface for playable clip classes forces a Play and Positional Play method.
 */

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;




public abstract class PlayableClip : ScriptableObject
{

    /// <summary> Gets the specs for this clip</summary>
    public abstract ClipSpecs GetSpecs();

    /// <summary>
    /// This test method is a little scuffed still not sure of a good way to do it.
    /// </summary>
    public virtual void Test()
    {
        // Create Temp Object and Components
        AudioSource source = new GameObject("Audio Test (DELETE ME)").AddComponent<AudioSource>();
        AudioHandler handler = source.AddComponent<AudioHandler>();

        // Setup Source
        ClipSpecs specs = GetSpecs();
        source.clip = specs.clip;
        source.volume = specs.volume;
        source.pitch = specs.pitch;

        source.Play();

        // Destroy temporary Object after the clips completion
        handler.StartCoroutine(DestroyAfterClip());
        IEnumerator DestroyAfterClip()
        {
            yield return new WaitForSecondsRealtime(specs.clip.length);
            DestroyImmediate(source.gameObject);
        }
    }



    public void Play() => AudioHandler.Play(GetSpecs());
    public void Play(Vector3 pos) => AudioHandler.PlayPositional(GetSpecs(), pos);
    public void Play(float fadeInTime, uint slot) => AudioHandler.PlayLooping(GetSpecs(), fadeInTime, slot);

    public AudioSource GetTestPlayer()
    {
        AudioSource found = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        if (found == null) throw new System.Exception("Audio Test Failed: No audio source on \"Main Camera\"!");

        return found;
    }
}
