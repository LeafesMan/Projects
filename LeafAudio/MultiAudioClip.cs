/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/27/23 
 *  
 *  Desc: A Playable Clip that plays one random clip from its' clips array.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Multi Clip", fileName = "MA_NewMultiAudioClip")]
public class MultiAudioClip : PlayableClip
{
    public PlayableClip[] clips;
    public PlayableClip GetRandClip() => clips[Random.Range(0, clips.Length)];
    public override void Test() => GetRandClip().Test();
    public override void Play() => GetRandClip().Play();
    public override void Play(Vector3 pos) => GetRandClip().Play(pos);

}
