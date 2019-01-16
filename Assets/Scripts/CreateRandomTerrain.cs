using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomTerrain : MonoBehaviour {

    public GameObject floorTile;
    public GameObject wallTile;
    [Header("Map Size")]
    public Vector3Int size;

    void OnValidate() {
        size.y = Mathf.Max(size.y, 1);
    }
    [Space]
    [Range(0,1)]
    public float wallProb = 0.1f;

    private Tile[,,] tileMap;
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
    
    public void GenerateTerrain() {
        tileMap = new Tile[size.x, size.y, size.z];
        GenerateFloor();
        GenerateWalls(wallProb);
    }

    private void GenerateFloor() {
        for (int col = 0; col < tileMap.GetLength(0); ++col) {
            for (int row = 0; row < tileMap.GetLength(2); ++row) {
                tileMap[col, 0, row] = Tile.Create(floorTile, new Vector3(transform.position.x + col, transform.position.y, transform.position.z + row), terrainParrent.transform).GetComponent<Tile>();
            }
        }
    }

    private void GenerateWalls(float prob) { // Iterate through each height added random tiles no adding tiles if not supported
        for (int height = 1; height < tileMap.GetLength(1); ++height) { // Iterate through each height first
            for (int col = 0; col < tileMap.GetLength(0); ++col) {
                for (int row = 0; row < tileMap.GetLength(2); ++row) {
                    if (Random.Range(0f,1f) < prob && tileMap[col, height, row] == null && tileMap[col, height-1, row] != null)
                        tileMap[col, height, row] = Tile.Create(wallTile, new Vector3(transform.position.x + col, transform.position.y + height, transform.position.z + row), terrainParrent.transform).GetComponent<Tile>();
                }
            }
        }
    }
}
