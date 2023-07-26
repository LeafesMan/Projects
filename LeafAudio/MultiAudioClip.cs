////////////////////////////////////
//
//  Author: Ian
//
//  Project: The Wall
//  
//  Date: 12/15/22
//       A data structure that stores multiple audio clips.
//      Allows us to change the Hurt MultiAudioClip affecting all Hurt MultiAudioClips in the game.
// 
////////////////////////////////////
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Multi Clip", fileName = "MA_NewMultiAudioClip")]
public class MultiAudioClip : ScriptableObject, IPlayableClip
{
    public VariableAudioClip[] clips;
    public VariableAudioClip GetRandClip() => clips[Random.Range(0, clips.Length)];
    public void TestClip() => GetRandClip().TestClip();
    public void Play() => GetRandClip().Play();
    public void Play(Vector3 pos) => GetRandClip().Play(pos);

}
