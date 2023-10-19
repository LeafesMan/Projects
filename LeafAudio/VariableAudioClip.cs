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
    public AudioClipSpecs[] clips;

    [System.Serializable]
    public struct AudioClipSpecs 
    {
        public AudioClip clip;
        [SerializeField, Slider(0, 1)] private Vector2 volumeRange;
        [SerializeField, Slider(0, 3)] private Vector2 pitchRange;
        public float GetRandVolume() => Random.Range(volumeRange.x, volumeRange.y);
        public float GetRandPitch() => Random.Range(pitchRange.x, pitchRange.y);
    }

    AudioClipSpecs GetRandomSpecs() => clips[Random.Range(0, clips.Length)];
    public override void Test()
    {
        AudioSource source = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        AudioClipSpecs toPlay = GetRandomSpecs();
        source.clip = toPlay.clip;
        source.volume = toPlay.GetRandVolume();
        source.pitch = toPlay.GetRandPitch();
        source.Play();
    }

    public override void Play()
    {
        AudioClipSpecs toPlay = GetRandomSpecs();
        AudioHandler.PlayClip(toPlay.clip, toPlay.GetRandVolume(), toPlay.GetRandPitch());
    }
    public override void Play(Vector3 pos)
    {
        AudioClipSpecs toPlay = GetRandomSpecs();

        AudioHandler.PlayClipPositional(toPlay.clip, toPlay.GetRandVolume(), toPlay.GetRandPitch(), pos);
    }
}
