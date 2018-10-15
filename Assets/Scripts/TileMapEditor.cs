using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileMapEditor : MonoBehaviour {

    public Tile[] tiles;
    //public HashSet<Tile> createdTiles = new HashSet<Tile>();
    
    private GameObject _terrainParent;
    private GameObject terrainParrent {
        get {
            if (_terrainParent != null)
                return _terrainParent;
            _terrainParent = GameObject.Find("TerrainEditor");
            if (_terrainParent == null) {
                _terrainParent = new GameObject("TerrainEditor");
            }
            return _terrainParent;
        }
    }

    public void CreateTile(Vector3 pos) {
        GameObject g = Instantiate<GameObject>(tiles[0].gameObject, pos, Quaternion.identity, terrainParrent.transform);
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
}
