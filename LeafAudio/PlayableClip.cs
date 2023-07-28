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
public abstract class PlayableClip : ScriptableObject
{
	public abstract void Play();
	public abstract void Play(Vector3 pos);
	public abstract void Test();
}
