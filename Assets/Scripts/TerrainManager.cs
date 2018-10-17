﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3Extensions;

public class TerrainManager : MonoBehaviour {

    public Tile[,,] tileMap;
    public float timer;
    private string pastLevel = null;

    private static TerrainManager _terrainManager;
    public static TerrainManager terrainManager {
        get {
            if (_terrainManager != null)
                return _terrainManager;
            _terrainManager = FindObjectOfType<TerrainManager>();
            if (_terrainManager == null) {
                Debug.LogError("Terrain Manager not found, Created new Terrain Manager.");
                _terrainManager = new GameObject("Terrain Manager").AddComponent<TerrainManager>();
            }
            return _terrainManager;
        }
    }

    [Header("Stage Collapse Settings")]
    [Tooltip("Time it takes for stage to fully destroy")]
    public float collapseTime = 30;
    [Tooltip("Radius of area where collapsing is happening")]
    public float collapseBuffer = 1;
    [Tooltip("Time warning will show before block is destroyed")]
    public float warningTimer = 1;
    [Tooltip("Rate of checking tiles to destory, lower the laggier")]
    [Range(0.02f, 1f)]
    public float updateRate = 0.1f; // Set the update rate

    // Use this for initialization
    void Start () {
        StartGame();
	}

    public void StartGame() {
        tileMap = ReadTileMap(); // Read Tile Map currently in scene into memory
        if (tileMap == null || tileMap.Length == 0) { // If no map, create a random map
            CreateRandomTerrain c = GetComponent<CreateRandomTerrain>();
            if (c != null)
                c.GenerateTerrain();
            else
                Debug.Log("No tile Map loaded and Random Terrain Component not included");
        }

        Invoke("StartCountdown", timer);
    }

    public void StartCountdown() {
        StartCoroutine("CollapseTerrain", collapseTime);
    }

    private IEnumerator CollapseTerrain(float timer) { // Process destroying the terrain over a set time
        float endTimer = Time.time;
        Vector3 center = new Vector3((tileMap.GetLength(0) - 1) / 2f, 0, (tileMap.GetLength(2) - 1) / 2f); // diameter == numBlocks-1  EX: dist between 2 blocks is 1
        Vector3 radius = center + new Vector3(collapseBuffer, 0, collapseBuffer); // Add buffer to radius
        Vector3 tPerS = radius / timer; // Tiles covered per sec
        float ratio; // declare outside for while loop condition
        do {
            ratio = 1 - (Time.time - endTimer) / timer; // ratio to shrink stage based on timer
            Vector3 newRadius = radius * ratio;

            for (int col = 0; col < tileMap.GetLength(0); ++col) { // check chance to destroy for every block
                for (int row = 0; row < tileMap.GetLength(2); ++row) {
                    Vector3 distToRing = (new Vector3(col, 0, row) - center).Abs() - newRadius; // Tile distance from collapsing area
                    Vector3 tileProb = (distToRing + new Vector3(collapseBuffer, 0, collapseBuffer)) / (collapseBuffer * 2); // Probability of tile collapsing based on distance from ring of collapse
                    if (Random.Range(0f, 1f) < Mathf.Max(tileProb.x * tPerS.x, tileProb.z * tPerS.z, 0) * updateRate * 4 || Mathf.Max(distToRing.x, distToRing.z) > collapseBuffer) // Check chance to collapse or if block is too far out
                        for (int height = 0; height < tileMap.GetLength(1); ++height)
                            if (tileMap[col, height, row] != null)
                                tileMap[col, height, row].Destroy(warningTimer); // Destroy tile after warning time
                }
            }

            if (Mathf.Min(newRadius.x, newRadius.z) < -collapseBuffer)
                break; // quit loop when all tiles are for sure destroyed
            DebugShrinkTerrain(center + transform.position, newRadius); // Display visual area of where shrinking is happening in editor
            yield return new WaitForSeconds(updateRate);
        } while (ratio > -0.2); // quit loop if something goes wrong
        LoadLevel("TowerLevel");
    }

    private void DebugShrinkTerrain(Vector3 center, Vector3 radius) { // Display visual area of where shrinking is happening in editor
        Vector3 botleft = center - radius - new Vector3(collapseBuffer, 0, collapseBuffer) + Tile.TileOffset;
        Vector3 topRight = center + radius + new Vector3(collapseBuffer, 0, collapseBuffer) + Tile.TileOffset;
        Debug.DrawLine(botleft, new Vector3(botleft.x, botleft.y, topRight.z), Color.red, 0.1f, false);
        Debug.DrawLine(topRight, new Vector3(botleft.x, botleft.y, topRight.z), Color.red, 0.1f, false);
        Debug.DrawLine(topRight, new Vector3(topRight.x, botleft.y, botleft.z), Color.red, 0.1f, false);
        Debug.DrawLine(botleft, new Vector3(topRight.x, botleft.y, botleft.z), Color.red, 0.1f, false);
        botleft += 2 * new Vector3(collapseBuffer, 0, collapseBuffer);
        topRight -= 2 * new Vector3(collapseBuffer, 0, collapseBuffer);
        Debug.DrawLine(botleft, new Vector3(botleft.x, botleft.y, topRight.z), Color.green, 0.1f, false);
        Debug.DrawLine(topRight, new Vector3(botleft.x, botleft.y, topRight.z), Color.green, 0.1f, false);
        Debug.DrawLine(topRight, new Vector3(topRight.x, botleft.y, botleft.z), Color.green, 0.1f, false);
        Debug.DrawLine(botleft, new Vector3(topRight.x, botleft.y, botleft.z), Color.green, 0.1f, false);
    }
    
    public Tile[,,] ReadTileMap() {
        GameObject g = GameObject.Find("Tile Map");
        if (g != null)
            return TileMap.ReadMap(g.transform);
        return null;
    }

    public void LoadLevel(string name) { // Removes the old level and loads in new level
        GameObject g = GameObject.Find("Tile Map");
        if (g != null)
            Destroy(g);
        if (pastLevel != null)
            DeleteOldLevel();
        pastLevel = name;
        StartCoroutine("LoadLevelAsync", name);
    }

    private IEnumerator LoadLevelAsync(string name) { // Loads scene and starts game once finished
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone) {
            yield return new WaitForEndOfFrame();
        }
        StartGame();
    }

    private void DeleteOldLevel() { // Remove old scene
        SceneManager.UnloadSceneAsync(pastLevel);
    }
}
