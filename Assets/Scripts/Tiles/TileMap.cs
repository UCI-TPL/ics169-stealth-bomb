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
        Bounds mapBounds = new Bounds();
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null)
                mapBounds.Encapsulate(tile.position.Round());
        }
        mapSize = mapBounds.size.Round() + Vector3Int.one;
        Vector3Int offset = mapBounds.min.Round();
        Tile[,,] tiles = new Tile[mapSize.x, mapSize.y, mapSize.z];
        foreach (Transform t in parentContainer) {
            Tile tile = t.GetComponent<Tile>();
            if (tile != null) {
                Vector3Int pos = tile.position.Round() - offset;
                tiles[pos.x, pos.y, pos.z] = tile;
                switch(tile.Type) {
                    case Tile.TileType.SpawnPoint:
                        SpawnTiles.Add((SpawnTile)tile);
                        break;
                    case Tile.TileType.Item:
                        ItemTiles.Add((ItemTile)tile);
                        break;
                }
            }
        }
        return tiles;
    }

    /// <summary>
    /// Searches through the tile map for all tiles in the y axis for a given x and z cord
    /// </summary>
    /// <param name="x"> X position in the TileMap </param>
    /// <param name="z"> Z position in the TileMap </param>
    /// <param name="startHeight"> Starting height to search from </param>
    /// <param name="checkNull"> Whether null tiles should be ignored </param>
    /// <returns> Array of all Tiles with the given cords </returns>
    public Tile[] GetPillar(int x, int z, int startHeight = 0, bool checkNull = false) {
        if (checkNull) {
            List<Tile> result = new List<Tile>();
            for (int height = startHeight; height < Tiles.GetLength(1); ++height) {
                if (Tiles[x, height, z] != null)
                    result.Add(Tiles[x, height, z]);
            }
            return result.ToArray();
        } else {
            Tile[] result = new Tile[Mathf.Max(Tiles.GetLength(1) - startHeight, 0)];
            for (int height = startHeight; height < Tiles.GetLength(1); ++height) {
                result[height - startHeight] = Tiles[x, height, z];
            }
            return result;
        }
    }
}
