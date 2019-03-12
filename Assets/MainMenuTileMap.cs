using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTileMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Color[] colorArray = new Color[] { new Color(0, 0, 0, 1) };
        Texture2D t = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        t.SetPixels(colorArray);
        t.Apply();
        Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), t);
        Shader.SetGlobalVector(Shader.PropertyToID("_TileMapSize"), Vector3.one);
    }
}
