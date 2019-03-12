using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class FixTileDamageEditor {
    static FixTileDamageEditor() {
        EditorApplication.update += Update;
    }

    static void Update() {
        if (!EditorApplication.isPlaying) {
            Color[] colorArray = new Color[] { new Color(0, 0, 0, 1) };
            Texture2D t = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
            t.SetPixels(colorArray);
            t.Apply();
            Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), t);
            Shader.SetGlobalVector(Shader.PropertyToID("_TileMapSize"), Vector3.one);
        }
    }
}


