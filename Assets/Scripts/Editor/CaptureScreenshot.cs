using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CaptureScreenshot {

    [MenuItem("Tools/Screenshot")]
    private static void Screenshot() {
        Camera main = Camera.main;
        RenderTexture rt = new RenderTexture(main.pixelWidth, main.pixelHeight, 16, RenderTextureFormat.ARGB32);
        main.targetTexture = rt;
        main.Render();
        RenderTexture.active = rt;
        Texture2D virtualPhoto = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        virtualPhoto.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null; //can help avoid errors
        main.targetTexture = null;

        byte[] bytes;
        bytes = virtualPhoto.EncodeToPNG();
        string path = "Screenshots/screenshot.png";
        Debug.Log("Saving Screenshot to: " + path);
        System.IO.File.WriteAllBytes(path, bytes);
    }
}
