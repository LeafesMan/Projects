/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/26/23 
 *  
 *  Desc: Interface for playable clip classes forces a Play and Positional Play method.
 */

using UnityEngine;
public interface IPlayableClip
{
	public abstract void Play();
	public abstract void Play(Vector3 pos);
	public abstract void Test();
	public static AudioSource GetTestPlayer()
	{
		AudioSource found = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		if (found == null) throw new System.Exception("Audio Test Failed: No audio source on \"Main Camera\"!");

		return found;
    }
}
