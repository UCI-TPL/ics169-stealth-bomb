using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class TileMap {

    public Tile[,,] Tiles { get; private set; }
    public List<Tile> SpawnTiles { get; private set; }
    public List<Tile> ItemTiles { get; private set; }

    public TileMap(Transform parentContainer) {
        SpawnTiles = new List<Tile>();
        ItemTiles = new List<Tile>();
        Tiles = ReadMap(parentContainer);
    }

    public Tile[,,] ReadMap(Transform parentContainer) {
        Vector3Int mapSize = Vector3Int.one * -1;
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null)
                mapSize = Max(mapSize, tile.position.Round());
        }
        mapSize += Vector3Int.one;
        Tile[,,] tiles = new Tile[mapSize.x, mapSize.y, mapSize.z];
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null) {
                Vector3Int pos = tile.position.Round();
                tiles[pos.x, pos.y, pos.z] = tile;
                switch(tile.type) {
                    case Tile.Type.SpawnPoint:
                        SpawnTiles.Add(tile);
                        break;
                    case Tile.Type.Item:
                        ItemTiles.Add(tile);
                        break;
                }
            }
        }
        return tiles;
    }

    private static Vector3Int Max(Vector3Int v1, Vector3Int v2) {
        return new Vector3Int(Mathf.Max(v1.x, v2.x), Mathf.Max(v1.y, v2.y), Mathf.Max(v1.z, v2.z));
    }
}
