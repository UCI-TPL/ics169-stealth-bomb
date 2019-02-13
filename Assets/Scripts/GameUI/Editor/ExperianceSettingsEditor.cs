using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperianceSettings))]
public class ExperianceSettingsEditor : Editor {

    private ExperianceSettings script {
        get { return (ExperianceSettings)target; }
    }

    SerializedProperty ExperianceTypes;

    private void OnEnable() {
        // Setup the SerializedProperties.
        ExperianceTypes = serializedObject.FindProperty("ExperianceTypes");
        // Populate the array of experiance types with different Experiance Types from enum
        while (ExperianceTypes.arraySize < Enum.GetNames(typeof(GameManager.GameRound.BonusExperiance.ExperianceType)).Length) {
            int index = ExperianceTypes.arraySize;
            ExperianceTypes.InsertArrayElementAtIndex(ExperianceTypes.arraySize);
            // Fill in the name with name of enum value
            ExperianceTypes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue = Enum.GetName(typeof(GameManager.GameRound.BonusExperiance.ExperianceType), index);
            ExperianceTypes.GetArrayElementAtIndex(index).FindPropertyRelative("Color").colorValue = Color.white;
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    public override void OnInspectorGUI() {
        GUIStyle BoldLabel = new GUIStyle(GUI.skin.label);
        BoldLabel.fontStyle = FontStyle.Bold;
        for (int i = 0; i < ExperianceTypes.arraySize; ++i) {
            SerializedProperty itemAtIndex = ExperianceTypes.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(Enum.GetName(typeof(GameManager.GameRound.BonusExperiance.ExperianceType), i), BoldLabel);
            EditorGUILayout.PropertyField(itemAtIndex.FindPropertyRelative("Name"));
            itemAtIndex.FindPropertyRelative("Color").colorValue = EditorGUILayout.ColorField(new GUIContent("Color"), itemAtIndex.FindPropertyRelative("Color").colorValue, true, false, false);
            EditorGUILayout.PropertyField(itemAtIndex.FindPropertyRelative("Points"));
            EditorGUILayout.PropertyField(itemAtIndex.FindPropertyRelative("Sprite"));
            EditorGUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
