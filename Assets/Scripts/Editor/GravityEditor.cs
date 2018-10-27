using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GravityController))]
[CanEditMultipleObjects]
public class GravityEditor : Editor {

    SerializedProperty gravityProperty;

    private void OnEnable()
    {
        gravityProperty = serializedObject.FindProperty("gravity");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        gravityProperty.floatValue = EditorGUILayout.FloatField("Gravity", gravityProperty.floatValue);
        ApplyGravity(gravityProperty.floatValue);
        serializedObject.ApplyModifiedProperties();
       
    }

    void ApplyGravity(float gravity)
    {
        Physics.gravity = new Vector3(0, gravity, 0);
    }
}
