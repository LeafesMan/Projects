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
public class MultiAudioClip : Audio
{
    public VariableAudioClip[] clips;
    public override VariableAudioClip GetClip() => clips[Random.Range(0, clips.Length)];
    public void TestClip() => GetClip().TestClip();
}
