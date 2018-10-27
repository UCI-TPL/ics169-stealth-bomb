using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerupList))]
public class PowerupListEditor : Editor {

    public static readonly Color[] colors =
        {
            new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
            new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
            new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };
    private PowerupList script {
        get { return (PowerupList)target; }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        float width = Screen.width-28;

        string errorString;
        float totalPercent = script.TotalPercent();
        errorString = totalPercent == 100 ? "" : "WARNING: Tiers add to " + totalPercent + "%";

        GUIStyle backgroundStyle = new GUIStyle();
        backgroundStyle.padding = new RectOffset(0, 0, 0, 0);
        backgroundStyle.normal.background = MakeTex(new Color(0.6f, 0.6f, 0.6f, 1f));
        GUILayout.BeginHorizontal(backgroundStyle, GUILayout.MaxHeight(50), GUILayout.Width(width));
        for (int i = 0; i < script.tiers.Length; ++i) {
            HorizontalBox(colors[i % colors.Length], width * (script.tiers[i].percent / totalPercent));
        }
        GUILayout.EndHorizontal();
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = Color.red;
        EditorGUILayout.LabelField(errorString, labelStyle);
    }

    private void HorizontalBox(Color color, float width) {
        GUIStyle box = new GUIStyle();
        box.normal.background = MakeTex(color);
        GUILayout.BeginHorizontal(box, GUILayout.Width(width));
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private Texture2D MakeTex(Color color) {
        Color[] pix = new Color[1];
        pix[0] = color;
        Texture2D result = new Texture2D(1, 1);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
