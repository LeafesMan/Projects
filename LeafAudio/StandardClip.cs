/*
 * Auth: Ian
 * 
 * Proj: Untitled Moster Game
 * 
 * Date: 10/19/23
 * 
 * Desc: A standard clip plays an audioclip with a set volume and pitch
 */

using UnityEngine;

public class StandardClip : ScriptableObject, IPlayableClip
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume;
    [SerializeField] float pitch;


    public void Test()
    {
        AudioSource source = IPlayableClip.GetTestPlayer();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    public void Play() => AudioHandler.PlayClip(clip, volume, pitch);
    public void Play(Vector3 pos) => AudioHandler.PlayClipPositional(clip, volume, pitch, pos);
}
