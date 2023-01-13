////////////////////////////////////
//
//  Author: Ian :)
//
//  Project: The Wall
//  
//  Date: 6/24/22
//      Allows for playing audio in editor with button
//
////////////////////////////////////
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VariableAudioClip))]
public class VariableAudioClipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        VariableAudioClip script = (VariableAudioClip)target;

        if (GUILayout.Button("Test Audio"))
        {
            Debug.Log("Testing Clip!");

            script.TestClip();
        }

    }
}
