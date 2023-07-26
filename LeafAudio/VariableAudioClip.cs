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
public class VariableAudioClip : ScriptableObject, IPlayableClip
{
    public AudioClip clip;

    [Range(0.01f, 1)] public float minVol, maxVol;
    [Range(0.01f, 3)] public float minPitch, maxPitch;
    public float GetRandVolume() => Random.Range(minVol, maxVol);
    public float GetRandPitch() => Random.Range(minPitch, maxPitch);
    public void TestClip()
    {
        AudioSource source = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = GetRandVolume();
        source.pitch = GetRandPitch();
        source.Play();
    }

    public void Play() => AudioHandler.Play(clip, GetRandVolume(), GetRandPitch());

    public void Play(Vector3 pos) => AudioHandler.Play(clip, GetRandVolume(), GetRandPitch(), pos);
}
