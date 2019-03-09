using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Vector3Extensions;

public class TileManager : MonoBehaviour {

    public TileMap tileMap;
    private Scene pastLevel;

    private Vector2 center;
    private Vector2 radius;
    private Vector2 newRadius;
    public Rect mapArea {
        get { return new Rect(center - newRadius + new Vector2(collapseBuffer, collapseBuffer), 2 * (newRadius - new Vector2(collapseBuffer, collapseBuffer))); }
    }

    private static TileManager _tileManager;
    public static TileManager tileManager {
        get {
            if (_tileManager != null)
                return _tileManager;
            _tileManager = FindObjectOfType<TileManager>();
            if (_tileManager == null) {
                Debug.LogWarning("Tile Manager not found, Created new Tile Manager.");
                _tileManager = new GameObject("Tile Manager").AddComponent<TileManager>();
            }
            return _tileManager;
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
    [Tooltip("Delay between destruction of tiles in a column")]
    public float pillarDestroyDelay = 0.05f;

    [Header("Material Settings(Do Not Change)")]
    [SerializeField]
    private Material basicMaterial;

    public delegate void LoadSceneAction(Scene loadedScene);

    private Queue<TileDestroyCalc> TileDestroyQueue;

    private Texture2D tileDamageMap;

    public void StartGame() {
        tileMap = ReadTileMap(); // Read Tile Map currently in scene into memory
        if (tileMap == null || tileMap.Tiles.Length == 0) { // If no map, create a random map
            CreateRandomTerrain c = GetComponent<CreateRandomTerrain>();
            if (c != null) {
                c.GenerateTerrain();
                tileMap = ReadTileMap();
            }
            else
                Debug.Log("No tile Map loaded and Random Terrain Component not included");
        }
        center = new Vector2((tileMap.Tiles.GetLength(0) - 1) / 2f, (tileMap.Tiles.GetLength(2) - 1) / 2f); // diameter == numBlocks-1  EX: dist between 2 blocks is 1
        radius = center + new Vector2(collapseBuffer, collapseBuffer); // Add buffer to radius
        newRadius = radius;

        TileDestroyQueue = CreateTileMapDestroyCalc(tileMap.Tiles, center, collapseBuffer);

        StopAllCoroutines(); // Stop updates from previous round

        tileDamageMap = CreateTexture3D(tileMap.Size.x, tileMap.Size.y, tileMap.Size.z, Color.black);
        Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), tileDamageMap);
        Shader.SetGlobalVector(Shader.PropertyToID("_TileMapSize"), (Vector3)tileMap.Size);
        StartCoroutine(raiseMap(1.7f));
    }

    struct VelocityJob : IJobParallelFor {
        // By default containers are assumed to be read & write
        public NativeArray<Color> pixels;

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job same frame or next frame, but the job should do work deterministically
        // independent on when the job happens to run on the worker threads.
        public float deltaTime;

        // The code actually running on the job
        public void Execute(int i) {
            // Move the positions based on delta time and velocity
            pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, pixels[i].a + deltaTime);
        }
    }

    private IEnumerator raiseMap(float duration) {
        float endTime = Time.time + duration;
        Color[] colors = tileDamageMap.GetPixels();
        NativeArray<Color> pixels = new NativeArray<Color>(colors.Length, Allocator.Persistent);
        VelocityJob job;
        JobHandle jobHandle;
        while (endTime > Time.time) {
            colors = tileDamageMap.GetPixels();
            pixels.CopyFrom(colors);

            // Initialize the job data
            job = new VelocityJob() {
                deltaTime = Time.deltaTime / duration,
                pixels = pixels
            };

            jobHandle = job.Schedule(pixels.Length, 64);
            jobHandle.Complete();
            job.pixels.CopyTo(colors);

            tileDamageMap.SetPixels(colors);
            yield return null;
        }
        colors = tileDamageMap.GetPixels();
        pixels.CopyFrom(colors);

        // Initialize the job data
        job = new VelocityJob() {
            deltaTime = 1,
            pixels = pixels
        };

        jobHandle = job.Schedule(pixels.Length, 64);
        jobHandle.Complete();
        job.pixels.CopyTo(colors);

        tileDamageMap.SetPixels(colors);
        pixels.Dispose();
    }

    private void Update() {
        if (tileDamageMap != null) {
            tileDamageMap.Apply();
            Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), tileDamageMap);
        }
    }

    Texture2D CreateTexture3D(int xSize, int ySize, int zSize, Color color) {
        Color[] colorArray = new Color[xSize * ySize * zSize];
        for (int i = 0; i < colorArray.Length; ++i) {
            color.a = UnityEngine.Random.Range(0f, 0.5f);
            colorArray[i] = color;
        }
        Texture2D t = new Texture2D(xSize, ySize * zSize, TextureFormat.RGBA32, false, false);
        t.SetPixels(colorArray);
        t.Apply();
        return t;
    }

    public void SetTileDamage(Vector3Int position, float crumbleValue, float disolveValue, float damageValue) {
        tileDamageMap.SetPixel(position.x, position.y + (position.z * tileMap.Size.y), new Color(crumbleValue, disolveValue, damageValue, 1));
    }

    /// <summary>
    /// Damage a column of tiles starting from the given y position up
    /// </summary>
    /// <param name="position"> position of the bottom of the pillar </param>
    /// <param name="damageAmount"> amount of damage dealt to each tile </param>
    public void DamagePillar(Vector3Int position, float damageAmount) {
        StartCoroutine(DamagePillarCoroutine(0.02f));

        IEnumerator DamagePillarCoroutine(float delay) {
            // Get all tiles in a column above the given point
            foreach (Tile t in tileMap.GetPillar(position.x, position.z, position.y)) {
                if (t != null)
                    t.ApplyDamage(damageAmount); // Deal damage to each tile
                yield return new WaitForSeconds(delay);
            }
        }
    }

    // Begin shrinking the terrain
    public void StartCountdown() {
        StartCoroutine("CollapseCircle", collapseTime);
        //StartCoroutine("CollapseRectangle", collapseTime);
    }

    // Collapse the map in a shrinking circle over the timer parameter
    private IEnumerator CollapseCircle(float timer) {
        float endTimer = Time.time;
        do {
            float ratio = 1 - (Time.time - endTimer) / timer; // ratio to shrink stage based on timer
            GameManager.instance.GhostOffset = new Vector3( (1 - ratio) * GameManager.instance.GhostOffsetLimit, 0f,(1- ratio ) * -GameManager.instance.GhostOffsetLimit);
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

    // Collapse the map in a shrinking rectangle over the timer parameter
    private IEnumerator CollapseRectangle(float timer) { // Process destroying the terrain over a set time
        float endTimer = Time.time;
        Vector2 tPerS = radius / timer; // Tiles covered per sec
        float ratio; // declare outside for while loop condition
        do {
            ratio = 1 - (Time.time - endTimer) / timer; // ratio to shrink stage based on timer
            newRadius = radius * ratio;

            for (int col = 0; col < tileMap.Tiles.GetLength(0); ++col) { // check chance to destroy for every block
                for (int row = 0; row < tileMap.Tiles.GetLength(2); ++row) {
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
    }

    // Display a debug area in the scene view showing current state of map collapsing
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

    // Reads all the tiles and returns an order for when every tile will collapse
    private static Queue<TileDestroyCalc> CreateTileMapDestroyCalc(Tile[,,] tiles, Vector2 center, float bufferRadius) {
        TileDestroyCalc[] toSort = new TileDestroyCalc[tiles.GetLength(0) * tiles.GetLength(2)];
        for (int col = 0; col < tiles.GetLength(0); ++col) { // Loop through every X and Z position in the tile map
            for (int row = 0; row < tiles.GetLength(2); ++row) {
                Vector2Int pos = new Vector2Int(col, row);
                toSort[row * tiles.GetLength(0) + col] = new TileDestroyCalc(pos, (pos - center).magnitude + UnityEngine.Random.Range(-bufferRadius, bufferRadius)); // Get distance from center plus some noise
            }
        }
        Array.Sort(toSort);
        Queue<TileDestroyCalc> result = new Queue<TileDestroyCalc>();
        foreach (TileDestroyCalc t in toSort)
            result.Enqueue(t);
        return result;
    }
    
    public void DestroyTiles(Vector3 tilePos) //called by the ghostCursor to destory tiles. Currently kills a whole pillar, might just kill a tile next time
    {
        StartCoroutine(DestroyPillar((int)tilePos.x, (int)tilePos.z));
    }
    
    // Destroys all tiles in that z and x position
    public IEnumerator DestroyPillar(int col, int row) {
        foreach (Tile t in tileMap.GetPillar(col, row)) {
            if (t != null)
                t.Destroy(warningTimer); // Destroy tile after warning time
            yield return new WaitForSeconds(pillarDestroyDelay);
        }
    }

    // Returns the TileMap currently loading in the scene
    public TileMap ReadTileMap() {
        GameObject g = GameObject.Find("Tile Map");
        if (g != null)
            return new TileMap(g.transform);
        return null;
    }

    // Removes the old level and loads in new level
    public Scene LoadLevel(string name, LoadSceneAction OnFinishLoad) {
        GameObject g = GameObject.Find("Tile Map");
        if (g != null)
            Destroy(g);
        if (pastLevel.isLoaded)
            DeleteOldLevel();
        StartCoroutine(LoadLevelAsync(name, OnFinishLoad));
        return pastLevel;
    }

    // Loads scene and starts game once finished
    private IEnumerator LoadLevelAsync(string name, LoadSceneAction OnFinishLoad) {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }
        pastLevel = SceneManager.GetSceneByName(name);
        if (OnFinishLoad != null)
            OnFinishLoad.Invoke(pastLevel);
    }

    // Remove old scene
    private void DeleteOldLevel() {
        SceneManager.UnloadSceneAsync(pastLevel);
    }

    // Object containing calculations for determining what order tiles are destroyed in
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
