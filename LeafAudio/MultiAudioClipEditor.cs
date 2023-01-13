////////////////////////////////////
//
//  Author: Ian :)
//
//  Project: The Wall
//  
//  Date: 12/15/22
//      Allows for playing audio in editor with button
//
////////////////////////////////////
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiAudioClip))]
public class MultiAudioClipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MultiAudioClip script = (MultiAudioClip)target;

        if (GUILayout.Button("Test Audio"))
        {
            Debug.Log("Testing Clip!");
            script.TestClip();
        }

    }
}

