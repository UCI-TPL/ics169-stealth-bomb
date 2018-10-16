using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class TileMap {
    
    public static Tile[,,] ReadMap(Transform parentContainer) {
        Vector3Int mapSize = new Vector3Int();
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null)
                mapSize = Max(mapSize, tile.transform.localPosition.Round());
        }
        mapSize += Vector3Int.one;
        Tile[,,] tileMap = new Tile[mapSize.x, mapSize.y, mapSize.z];
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            Vector3Int pos = tile.transform.localPosition.Round();
            if (tile != null)
                tileMap[pos.x, pos.y, pos.z] = tile;
        }
        return tileMap;
    }

    public static void saveMap(Tile[,,] tileMap, string path) {

    }

    //private static Tile[,,] ReadMapRecursive(Transform p, int i, Vector3Int mapSize) {
    //    if (i >= p.childCount)
    //        return new Tile[mapSize.x, mapSize.y, mapSize.z];
    //    else {
    //        Transform child = p.GetChild(i);
    //        Tile[,,] t = ReadMapRecursive(p, ++i, Max(mapSize, child.localPosition.Round()));
    //        Vector3Int pos = p.GetChild(i).localPosition.Round();
    //        t[pos.x, pos.y, pos.z] = p.GetChild(i).GetComponent<Tile>();
    //        return t;
    //    }
    //}

    private static Vector3Int Max(Vector3Int v1, Vector3Int v2) {
        return new Vector3Int(Mathf.Max(v1.x, v2.x), Mathf.Max(v1.y, v2.y), Mathf.Max(v1.z, v2.z));
    }
}
