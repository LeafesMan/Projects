////////////////////////////////////
//
//  Author: Ian
//
//  Project: The Wall
//  
//  Date: 12/23/22
//       A Interface for audio place any
// 
////////////////////////////////////
using UnityEngine;
public enum AudioMaterial
{
    wood = 0,
    fabric = 1,
    stone = 2,
    steel = 3,
    flesh = 4
}
public abstract class Audio : ScriptableObject
{
    public abstract VariableAudioClip GetClip();



    //*********** PLAYS SOUND closest to ATTACK'S POSITION from HIT's POSITION ************\\
    public static void PlayHitAudio(EventHandler e, AudioMaterial atkAudioMat, Collider2D atkCol, Collider2D hitCol)
    {
        //Play profile audio based on the hit's material
        PlayHitAudio(e, atkAudioMat, hitCol.GetComponent<GeneralStats>().GetAudioMaterial(), hitCol.ClosestPoint(atkCol.bounds.center));
    }
    public static void PlayHitAudio(ActionEffectData effectData, Collider2D atkCol, Collider2D hitCol)
    {   //Plays Audio Profile's audio for Hit's Audio Material
        PlayHitAudio(effectData.eventHandler, effectData.audioMat, hitCol.GetComponent<GeneralStats>().GetAudioMaterial(), hitCol.ClosestPoint(atkCol.bounds.center));
    }

    //*********** PLAYS SOUND AT HIT'S POSITION ************\\
    public static void PlayHitAudio(BaseHitbox atkHitbox, BaseHitbox hitHitbox)
    {   //Play profile audio based on hitHitbox's material
        PlayHitAudio(atkHitbox.GetAcEffectData().eventHandler, atkHitbox.GetAcEffectData().audioMat, hitHitbox.GetAcEffectData().audioMat, atkHitbox.transform.position);
    }


    public static void PlayHitAudio(EventHandler eventHandler, AudioMaterial atkAudioMat, AudioMaterial hitAudioMat, Vector3 pos)
    {
        eventHandler.audioEvent(eventHandler.hitAudioMatrix.GetAudio(atkAudioMat, hitAudioMat), pos);
    }
}
