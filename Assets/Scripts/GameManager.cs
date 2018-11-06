using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require an inputManager
[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour {
    public static GameManager instance;

    [Header("Managers")]
    public InputManager inputManager;
    public PlayerJoinManager playerJoinManager;
    public TileManager tileManager;

    [Header("Important Scene Names")]
    public string mainMenuSceneName;

    private string currentSceneName;
    // Important Data from the Main Menu.
    // this is by default set to true for all values so that the players spawn as normal if we start in a level.
    private bool[] readyPlayers = { true, true, true, true };

    public Player PlayerPrefab;
    // Important Data in any non Main Menu scene.
    private Player[] players;

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
        tileManager = TileManager.tileManager;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoaded;
    }

    // The onSceneLoaded function is where you are going to do most of the data pushing from scene to scene.
    void onSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (instance == this)
        {
            currentSceneName = scene.name;
            if (scene.name == mainMenuSceneName)
            {
                if (playerJoinManager == null)
                    playerJoinManager = GameObject.FindGameObjectWithTag("player-join-manager").GetComponent<PlayerJoinManager>();
            }
            else if (scene.name != mainMenuSceneName)           // WE ARE ASSUMING ANYTHING THAT ISN'T THE MAIN MENU IS A LEVEL. THIS IS CLEARLY NOT GOING TO BE THE CASE AT ALL TIMES, SO UPDATE THIS AS NEEDED.
            {
                tileManager.StartGame();
                players = new Player[readyPlayers.Length];
                Queue<SpawnTile> spawnPoints = new Queue<SpawnTile>(tileManager.tileMap.SpawnTiles);
                for (int i = 0; i < readyPlayers.Length; ++i) {
                    if (readyPlayers[i]) {
                        SpawnTile spawnTile = spawnPoints.Dequeue();
                        Instantiate<GameObject>(PlayerPrefab.gameObject, spawnTile.transform.position, Quaternion.identity).GetComponent<Player>().playerNumber = i;
                    }
                }
            }
        }
    }

    // Update exists here to handle some things are need to be done during a scene, not when a scene is loaded.
    void Update()
    {
        if (currentSceneName == mainMenuSceneName)                                          // If the game manager is in the main menu...
        {
            if (playerJoinManager != null)                                                  // If there is a PlayerJoinManager in the scene...
            {
                readyPlayers = playerJoinManager.GetPLayerReadyStatusList();                // Have the GameManager store the players who are currently ready.
            }
        }
    }
}
