/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/27/23 
 *  
 *  Desc: A variable clip is the data structure for a series of varying clips that Contains ranges for volume and pitch.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Variable Clip", fileName = "VA_NewVariableClip")]
public class VariableAudioClip : ScriptableObject, IPlayableClip
{
    [SerializeField] AudioClipSpecs[] clips;

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
    public void Test()
    {
        AudioSource source = IPlayableClip.GetTestPlayer();
        AudioClipSpecs toPlay = GetRandomSpecs();
        source.clip = toPlay.clip;
        source.volume = toPlay.GetRandVolume();
        source.pitch = toPlay.GetRandPitch();
        source.Play();
    }

    public void Play()
    {
        AudioClipSpecs toPlay = GetRandomSpecs();
        AudioHandler.PlayClip(toPlay.clip, toPlay.GetRandVolume(), toPlay.GetRandPitch());
    }
    public void Play(Vector3 pos)
    {
        AudioClipSpecs toPlay = GetRandomSpecs();

        AudioHandler.PlayClipPositional(toPlay.clip, toPlay.GetRandVolume(), toPlay.GetRandPitch(), pos);
    }
}
