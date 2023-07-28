/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/27/23 
 *  
 *  Desc: A variable audio clip is the data structure for a single clip. Contains ranges for volume and pitch.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Variable Clip", fileName = "VA_NewVariableClip")]
public class VariableAudioClip : PlayableClip
{
    public AudioClip clip;

    [SerializeField,Slider(0, 1)] private Vector2 volumeRange;
    [SerializeField,Slider(0, 3)] private Vector2 pitchRange;
    private float GetRandVolume() => Random.Range(volumeRange.x, volumeRange.y);
    private float GetRandPitch() => Random.Range(pitchRange.x, pitchRange.y);
    public override void Test()
    {
        AudioSource source = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = GetRandVolume();
        source.pitch = GetRandPitch();
        source.Play();
    }

    public override void Play() => AudioHandler.PlayClipDelegate?.Invoke(clip, GetRandVolume(), GetRandPitch());
    public override void Play(Vector3 pos) => AudioHandler.PlayPositionalClipDelegate?.Invoke(clip, GetRandVolume(), GetRandPitch(), pos);
}
