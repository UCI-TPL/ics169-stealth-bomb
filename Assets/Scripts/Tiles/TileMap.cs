using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3Extensions;

public class TileMap {

    public Tile[,,] Tiles { get; private set; }
    public List<SpawnTile> SpawnTiles { get; private set; }
    public List<ItemTile> ItemTiles { get; private set; }

    public TileMap(Transform parentContainer) {
        SpawnTiles = new List<SpawnTile>();
        ItemTiles = new List<ItemTile>();
        Tiles = ReadMap(parentContainer);
        SpawnTiles = SpawnTiles.OrderByDescending(o => o.priority).ToList();
    }

    public Tile[,,] ReadMap(Transform parentContainer) {
        Vector3Int mapSize = Vector3Int.one * -1;
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null)
                mapSize = Vector3.Max(mapSize, tile.position.Round()).Round();
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
                        SpawnTiles.Add((SpawnTile)tile);
                        break;
                    case Tile.Type.Item:
                        ItemTiles.Add((ItemTile)tile);
                        break;
                }
            }
        }
        return tiles;
    }
}
