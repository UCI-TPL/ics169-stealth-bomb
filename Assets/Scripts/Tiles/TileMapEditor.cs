using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;
#if UNITY_EDITOR //Editor only tag
using UnityEditor;
#endif //Editor only tag

public class TileMapEditor : MonoBehaviour {
#if UNITY_EDITOR //Editor only tag
    public TileList tileList;
    //public HashSet<Tile> createdTiles = new HashSet<Tile>();
    public int selectTile = 0;
    public Bounds mapBounds = new Bounds();
    
    private GameObject _terrainParent;
    private GameObject terrainParrent {
        get {
            if (_terrainParent != null)
                return _terrainParent;
            _terrainParent = GameObject.Find("Tile Map");
            if (_terrainParent == null) {
                _terrainParent = new GameObject("Tile Map");
            }
            return _terrainParent;
        }
    }

    public void CreateTile(Vector3 pos) {
        GameObject g = Tile.PrefabCreate(tileList.tiles[selectTile].gameObject, pos, Quaternion.identity, terrainParrent.transform);
        Undo.RegisterCreatedObjectUndo(g, "Undo Create Tile");
        mapBounds.Encapsulate(g.GetComponent<Tile>().position);
    }

    public void DeleteTile(Tile tile) {
        Undo.DestroyObjectImmediate(tile.gameObject);
        RecalculateBounds();
    }

    public void ClearTiles() {
        //foreach (Tile t in createdTiles)
        //    Undo.DestroyObjectImmediate(t.gameObject);
        //createdTiles = new HashSet<Tile>();
        for (int i = terrainParrent.transform.childCount-1; i >= 0; --i)
            if (terrainParrent.transform.GetChild(i).GetComponent<Tile>() != null)
                Undo.DestroyObjectImmediate(terrainParrent.transform.GetChild(i).gameObject);
    }
    
    [UnityEditor.MenuItem("TileMapEditor/ResetCrumble")]
    private static void ResetCrumble() {
        Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), Texture2D.blackTexture);
        Shader.SetGlobalVector(Shader.PropertyToID("_TileMapSize"), Vector3.one);
    }

    public void RecalculateBounds() {
        mapBounds = new Bounds();
        foreach (Transform t in terrainParrent.transform) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null)
                mapBounds.Encapsulate(tile.position);
        }
    }
#endif //Editor only tag
}
