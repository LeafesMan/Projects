////////////////////////////////////
//
//  Author: Ian
//
//  Project: The Wall
//  
//  Date: 12/15/22
//       A variable audio clip is the data structure for a single clip.
//       Contains ranges for volume, pitch, and zzzt 
// 
////////////////////////////////////
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Variable Clip", fileName = "VA_NewVariableClip")]
public class VariableAudioClip : Audio
{
    public AudioClip clip;

    [Range(0.01f, 1)] public float minVol, maxVol;
    [Range(0.01f, 3)] public float minPitch, maxPitch;
    public override VariableAudioClip GetClip() => this;
    public float GetVolume() => Random.Range(minVol, maxVol);
    public float GetPitch() => Random.Range(minPitch, maxPitch);
    public void TestClip()
    {
        AudioSource source = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = GetVolume();
        source.pitch = GetPitch();
        source.Play();
    }
}
