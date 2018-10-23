using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerupData))]
public class PowerupEditor : Editor {

    private PowerupData script {
        get { return (PowerupData)target; }
    }

    private void OnEnable() {
        if (script.modifiers == null)
            script.modifiers = new List<PlayerStats.Modifier>();
    }

    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(75));
        script.name = EditorGUILayout.TextField(script.name);
        GUILayout.EndHorizontal();
        
        script.image = EditorGUILayout.ObjectField("Image", script.image, typeof(Texture2D), false) as Texture2D;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Description", GUILayout.Width(75));
        script.description = EditorGUILayout.TextArea(script.description, EditorStyles.textArea);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(GUI.skin.box); // Create a box containing properties
        GUIStyle title = new GUIStyle(GUI.skin.label); // Text style for title
        title.fontSize = 16;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Modifiers", title);
        GUILayout.Space(-7);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        for (int i = 0; i < script.modifiers.Count; ++i) {
            GUILayout.BeginHorizontal();
            script.modifiers[i].name = GUILayout.TextField(script.modifiers[i].name);
            script.modifiers[i].value = EditorGUILayout.FloatField(script.modifiers[i].value);
            GUIStyle style = new GUIStyle(GUI.skin.button); // Text style for title
            style.fontSize = 15;
            if (GUILayout.Button("-", style, GUILayout.MaxWidth(20), GUILayout.MaxHeight(18)))
                script.RemoveModifier(i);
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Modifier")) {
            script.AddModifier("", 0);
        }

        GUILayout.EndVertical();
        if (GUI.changed) {
            EditorUtility.SetDirty(script);
        }
    }
}
