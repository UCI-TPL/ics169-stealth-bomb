﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMapEditor))]
public class TileMapEditorInspector : Editor {

    SerializedProperty tiles;

    private bool editing = false;
    private TileMapEditor script {
        get { return (TileMapEditor)target; }
    }
    private Vector3 mousePosition;

    Tool LastTool = Tool.None;

    void OnEnable() {
        tiles = serializedObject.FindProperty("tiles");
        LastTool = Tools.current; // Save current tool to be able to restore later
    }

    private void OnDisable() {
        Tools.current = LastTool; // Ensure editor tool restores to what it was before use
    }

    private void OnSceneGUI() {
        if (editing) {
            Event e = Event.current;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (e.button == 0 && e.type == EventType.MouseDown) // Disables selecting other objects while editing, to avoid leaving editor
                GUIUtility.hotControl = controlId;

            mousePosition = e.mousePosition;
            Vector3 targetGrid = Vector3.zero;

            RaycastHit hit; // position at mouse cursor
            Tile tile; // Tile at mouse cursor
            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(mousePosition), out hit) && (tile = hit.collider.GetComponent<Tile>()) != null) {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual; // Set drawing to check depth, otherwise it would overlay
                if (!e.shift) { // When shift is not held
                    targetGrid = round(hit.point + hit.normal * 0.1f);
                    DrawCubeWithWire(targetGrid, 1, Color.white, new Color(1, 1, 1, 0.1f));
                    if (e.button == 0 && e.type == EventType.MouseDown) {
                        script.CreateTile(targetGrid);
                    }
                } else {        // When shift is held
                    targetGrid = round(hit.point - hit.normal * 0.1f);
                    DrawCubeWithWire(targetGrid, 1, Color.red, new Color(1, 0, 0, 0.25f));
                    if (e.button == 0 && e.type == EventType.MouseDown) {
                        script.DeleteTile(tile);
                    }
                }
            }

            if (e.isMouse) // Force scene to redraw GUI elements
                SceneView.RepaintAll();
        }
    }

    private void DrawCubeWithWire (Vector3 pos, float size, Color wireColor, Color cubeColor) {
        Handles.color = wireColor;
        Handles.DrawWireCube(pos, new Vector3(size, size, size));
        Handles.color = cubeColor;
        Handles.CubeHandleCap(0, pos, Quaternion.identity, size + 0.001f, EventType.Repaint); // slight offset on size to fix z-fighting

    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(tiles, true);
        serializedObject.ApplyModifiedProperties();

        // Display controls
        {
            GUILayout.BeginVertical(GUI.skin.box); // Create a box containing controls
            GUIStyle title = new GUIStyle(GUI.skin.label); // Text style for title
            title.fontSize = 16;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Controls", title);
            GUILayout.Space(-7);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(GUI.skin.label); // Text style for content
            style.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("Place Tile\nDelete Tile", style);
            GUILayout.Label(": Right Mouse Button\n: Shift + Right Mouse Button");
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();
        }

        // Edit Button
        {
            GUIStyle style = new GUIStyle(GUI.skin.button); // set button font size
            style.fontSize = 16;
            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = editing ? new Color(1, 0.25f, 0.25f) : Color.green; // set button color
            if (GUILayout.Button(editing ? "Stop Editing" : "Edit", style, GUILayout.MinHeight(36))) {
                editing = !editing;
                if (editing) {
                    LastTool = Tools.current; // Save current tool to be able to restore later
                    Tools.current = Tool.None; // remove current tool to hide UI handles
                } else
                    Tools.current = LastTool; // restore tool to before edit
            }
            GUI.backgroundColor = defaultColor; // reset gui color
        }

        // Clear Tiles button
        if (GUILayout.Button("clear tiles")) {
            script.ClearTiles();
        }
    }

    private Vector3 round(Vector3 v3) {
        return new Vector3(Mathf.Round(v3.x), Mathf.Round(v3.y), Mathf.Round(v3.z));
    }
}
