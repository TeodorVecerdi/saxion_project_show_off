using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class SaveRenderTexture : MonoBehaviour {
    public RenderTexture RenderTexture;

    [Button]
    public void Save() {
        var camera = GetComponent<Camera>();
        var tempRT = new RenderTexture(2048, 1024, 24, RenderTextureFormat.Default);
        camera.targetTexture = tempRT;
        camera.Render();
        RenderTexture.active = tempRT;
        var tex = new Texture2D(2048, 1024, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, 2048, 1024), 0, 0);
        RenderTexture.active = null;
        camera.targetTexture = null;

        var bytes = tex.EncodeToPNG();
        var path = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))}/{AssetDatabase.GetAssetPath(RenderTexture)}.png";
        System.IO.File.WriteAllBytes(path, bytes);        
    }
}