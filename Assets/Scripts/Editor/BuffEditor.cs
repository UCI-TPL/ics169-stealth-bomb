using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffData))]
public class BuffEditor : Editor {

    private BuffData script {
        get { return (BuffData)target; }
    }

    public override void OnInspectorGUI() {
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Name", GUILayout.Width(75));
        //script.name = EditorGUILayout.TextField(script.name);
        //GUILayout.EndHorizontal();

        //script.image = EditorGUILayout.ObjectField("Image", script.image, typeof(Sprite), false) as Sprite;

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Description", GUILayout.Width(75));
        //script.description = EditorGUILayout.TextArea(script.description, EditorStyles.textArea);
        //GUILayout.EndHorizontal();

        // Display modifier settings
        ModifierSettings();

        TriggerSettings();

        if (GUI.changed) {
            EditorUtility.SetDirty(script);
        }
        EditorUtility.SetDirty(script);
    }

    private void ModifierSettings() {
        GUILayout.BeginVertical(GUI.skin.box); // Create a box containing modifiers
        GUIStyle title = new GUIStyle(GUI.skin.label); // Text style for title
        title.fontSize = 16;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Modifiers", title);
        GUILayout.Space(-7);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        for (int i = 0; i < script.Modifiers.Count; ++i) {
            GUILayout.BeginHorizontal();
            script.Modifiers[i].name = EditorGUILayout.TextField(script.Modifiers[i].name);
            script.Modifiers[i].value = EditorGUILayout.FloatField(script.Modifiers[i].value);
            script.Modifiers[i].type = (PlayerStats.Modifier.ModifierType)EditorGUILayout.EnumPopup(script.Modifiers[i].type);
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
    }

    private void TriggerSettings() {
        GUILayout.BeginVertical(GUI.skin.box); // Create a box containing modifiers
        GUIStyle title = new GUIStyle(GUI.skin.label); // Text style for title
        title.fontSize = 16;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Triggers", title);
        GUILayout.Space(-7);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        for (int i = 0; i < script.Triggers.Count; ++i) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            script.Triggers[i].condition = (Buff.Trigger.TriggerCondition)EditorGUILayout.EnumPopup("Condition", script.Triggers[i].condition);
            script.Triggers[i].cooldown = EditorGUILayout.FloatField("Cooldown", script.Triggers[i].cooldown);
            script.Triggers[i].triggerWeapon = (WeaponData)EditorGUILayout.ObjectField(script.Triggers[i].triggerWeapon, typeof(WeaponData), false);
            EditorGUILayout.EndVertical();

            GUIStyle style = new GUIStyle(GUI.skin.button); // Text style for title
            style.fontSize = 15;
            if (GUILayout.Button("-", style, GUILayout.MaxWidth(20), GUILayout.MaxHeight(18)))
                script.RemoveTrigger(i);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Trigger")) {
            script.AddTrigger();
        }

        //for (int i = 0; i < script.Triggers.Count; ++i) {
        //    GUILayout.BeginHorizontal();
        //    script.modifiers[i].name = GUILayout.TextField(script.modifiers[i].name);
        //    script.modifiers[i].value = EditorGUILayout.FloatField(script.modifiers[i].value);
        //    GUIStyle style = new GUIStyle(GUI.skin.button); // Text style for title
        //    style.fontSize = 15;
        //    if (GUILayout.Button("-", style, GUILayout.MaxWidth(20), GUILayout.MaxHeight(18)))
        //        script.RemoveModifier(i);
        //    GUILayout.EndHorizontal();
        //}

        //if (GUILayout.Button("Add Modifier")) {
        //    script.AddModifier("", 0);
        //}

        GUILayout.EndVertical();
    }
}
