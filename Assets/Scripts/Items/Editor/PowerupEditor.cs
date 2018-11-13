using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerupData))]
public class PowerupEditor : Editor {

    private PowerupData script {
        get { return (PowerupData)target; }
    }

    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(75));
        script.name = EditorGUILayout.TextField(script.name);
        GUILayout.EndHorizontal();
        
        script.image = EditorGUILayout.ObjectField("Image", script.image, typeof(Sprite), false) as Sprite;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Description", GUILayout.Width(75));
        script.description = EditorGUILayout.TextArea(script.description, EditorStyles.textArea);
        GUILayout.EndHorizontal();

        if (GUI.changed) {
            EditorUtility.SetDirty(script);
        }

        DrawDefaultInspector();
    }
}
