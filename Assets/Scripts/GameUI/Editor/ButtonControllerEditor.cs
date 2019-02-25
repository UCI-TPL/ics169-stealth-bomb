using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
// using UnityEditor.UI;

[CustomEditor(typeof(ButtonController))]
public class ButtonControllerEditor : Editor
{
    public override void OnInspectorGUI() {
        ButtonController targetButton = (ButtonController) target;
        // base.OnInspectorGUI();
        base.DrawDefaultInspector();
    }
}
