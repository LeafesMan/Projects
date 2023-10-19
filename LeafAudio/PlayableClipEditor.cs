/*
 *  Name: Ian
 *
 *  Proj: Audio Library
 *
 *  Date: 7/27/23 
 *  
 *  Desc: Allows for testing Playable Clips with a button.
 */

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(IPlayableClip), true)]
public class PlayableClipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IPlayableClip playableClipScript = (IPlayableClip)target;

        //Test Clip Button
        if (GUILayout.Button("Test Audio"))
        {
            Debug.Log("Testing Clip!");
            playableClipScript.Test();
        }
    }
}
