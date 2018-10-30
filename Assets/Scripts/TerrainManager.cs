using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3Extensions;

public class TerrainManager : MonoBehaviour {

    public Tile[,,] tileMap;
    public float timer;
    private string pastLevel = null;

    private Vector2 center;
    private Vector2 radius;
    private Vector2 newRadius;
    public Rect mapArea {
        get { return new Rect(center - newRadius + new Vector2(collapseBuffer, collapseBuffer), 2 * (newRadius - new Vector2(collapseBuffer, collapseBuffer))); }
    }

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

    private Queue<TileDestroyCalc> TileDestroyQueue;

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
        center = new Vector2((tileMap.GetLength(0) - 1) / 2f, (tileMap.GetLength(2) - 1) / 2f); // diameter == numBlocks-1  EX: dist between 2 blocks is 1
        radius = center + new Vector2(collapseBuffer, collapseBuffer); // Add buffer to radius
        newRadius = radius;

        TileDestroyQueue = CreateTileMapDestroyCalc(tileMap, center, collapseBuffer);

        Invoke("StartCountdown", timer);
    }

    public void StartCountdown() {
        StartCoroutine("CollapseCircle", collapseTime);
        //StartCoroutine("CollapseTerrain", collapseTime);
    }

    private IEnumerator CollapseCircle(float timer) {
        float endTimer = Time.time;
        do {
            float ratio = 1 - (Time.time - endTimer) / timer; // ratio to shrink stage based on timer
            newRadius = radius * ratio;
            while (TileDestroyQueue.Peek().destroyDistance > newRadius.x) {
                Vector2Int v = TileDestroyQueue.Dequeue().tilePos;
                StartCoroutine(DestroyPillar(v.x, v.y));
                if (TileDestroyQueue.Count <= 0) // Check after dequeue for just a little be more performance
                    break;
            }
            yield return null;
#if UNITY_EDITOR //Editor only tag
            DebugShrinkTerrain(new Vector3(center.x, 0, center.y) + transform.position, new Vector3(newRadius.x, 0, newRadius.y)); // Display visual area of where shrinking is happening in editor
#endif //Editor only tag
        } while (TileDestroyQueue.Count > 0 && newRadius.x >= -collapseBuffer);
    }

    private IEnumerator CollapseTerrain(float timer) { // Process destroying the terrain over a set time
        float endTimer = Time.time;
        Vector2 tPerS = radius / timer; // Tiles covered per sec
        float ratio; // declare outside for while loop condition
        do {
            ratio = 1 - (Time.time - endTimer) / timer; // ratio to shrink stage based on timer
            newRadius = radius * ratio;

            for (int col = 0; col < tileMap.GetLength(0); ++col) { // check chance to destroy for every block
                for (int row = 0; row < tileMap.GetLength(2); ++row) {
                    Vector2 distToRing = (new Vector2(col, row) - center).Abs() - newRadius; // Tile distance from collapsing area
                    Vector2 tileProb = (distToRing + new Vector2(collapseBuffer, collapseBuffer)) / (collapseBuffer * 2); // Probability of tile collapsing based on distance from ring of collapse
                    if (UnityEngine.Random.Range(0f, 1f) < Mathf.Max(tileProb.x * tPerS.x, tileProb.y * tPerS.y, 0) * updateRate * 4 || Mathf.Max(distToRing.x, distToRing.y) > collapseBuffer) // Check chance to collapse or if block is too far out
                        StartCoroutine(DestroyPillar(col, row));
                }
            }

            if (Mathf.Min(newRadius.x, newRadius.y) < -collapseBuffer)
                break; // quit loop when all tiles are for sure destroyed
            #if UNITY_EDITOR //Editor only tag
            DebugShrinkTerrain(new Vector3(center.x, 0, center.y) + transform.position, new Vector3(newRadius.x, 0, newRadius.y)); // Display visual area of where shrinking is happening in editor
            #endif //Editor only tag
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

    private static Queue<TileDestroyCalc> CreateTileMapDestroyCalc(Tile[,,] tileMap, Vector2 center, float bufferRadius) {
        TileDestroyCalc[] toSort = new TileDestroyCalc[tileMap.GetLength(0) * tileMap.GetLength(2)];
        for (int col = 0; col < tileMap.GetLength(0); ++col) { // Loop through every X and Z position in the tile map
            for (int row = 0; row < tileMap.GetLength(2); ++row) {
                Vector2Int pos = new Vector2Int(col, row);
                toSort[row * tileMap.GetLength(0) + col] = new TileDestroyCalc(pos, (pos - center).magnitude + UnityEngine.Random.Range(-bufferRadius, bufferRadius)); // Get distance from center plus some noise
            }
        }
        Array.Sort(toSort);
        Queue<TileDestroyCalc> result = new Queue<TileDestroyCalc>();
        foreach (TileDestroyCalc t in toSort)
            result.Enqueue(t);
        return result;
    }

    private IEnumerator DestroyPillar(int col, int row) {
        for (int height = 0; height < tileMap.GetLength(1); ++height) {
            if (tileMap[col, height, row] != null)
                tileMap[col, height, row].Destroy(warningTimer); // Destroy tile after warning time
            yield return new WaitForSeconds(0.05f);
        }
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

    private class TileDestroyCalc : IComparable {
        public float destroyDistance;
        public Vector2Int tilePos;

        public TileDestroyCalc(Vector2Int tilePos, float destroyDistance) {
            this.tilePos = tilePos;
            this.destroyDistance = destroyDistance;
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;
            TileDestroyCalc other = (TileDestroyCalc)obj;
            if (other != null)
                return other.destroyDistance.CompareTo(destroyDistance);
            else
                throw new ArgumentException("Object is not a TileDestroyCalc");
        }
    }
}
