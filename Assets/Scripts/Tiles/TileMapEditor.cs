using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR //Editor only tag
using UnityEditor;
#endif //Editor only tag

public class TileMapEditor : MonoBehaviour {
#if UNITY_EDITOR //Editor only tag
    public TileList tileList;
    //public HashSet<Tile> createdTiles = new HashSet<Tile>();
    public int selectTile = 0;
    public Vector3Int mapsize;
    
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
        //createdTiles.Add(g.GetComponent<Tile>());
    }

    public void DeleteTile(Tile tile) {
        //createdTiles.Remove(tile);
        Undo.DestroyObjectImmediate(tile.gameObject);
    }

    public void ClearTiles() {
        //foreach (Tile t in createdTiles)
        //    Undo.DestroyObjectImmediate(t.gameObject);
        //createdTiles = new HashSet<Tile>();
        for (int i = terrainParrent.transform.childCount-1; i >= 0; --i)
            if (terrainParrent.transform.GetChild(i).GetComponent<Tile>() != null)
                Undo.DestroyObjectImmediate(terrainParrent.transform.GetChild(i).gameObject);
    }
#endif //Editor only tag
}
