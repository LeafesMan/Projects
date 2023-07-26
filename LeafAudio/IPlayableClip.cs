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
	public void Play();
	public void Play(Vector3 pos);
}
