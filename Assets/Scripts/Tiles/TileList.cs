using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile List", menuName = "Tiles/Tile List", order = 1)]
public class TileList : ScriptableObject {
    // List of Tile prefabs, used for spawning tiles

    [SerializeField]
    private List<Tile> _tiles;
    public List<Tile> tiles {
        get {
            if (_tiles != null)
                return _tiles;
            _tiles = new List<Tile>();
            return _tiles;
        }
        set { _tiles = value; }
    }
}
