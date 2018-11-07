using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Require an inputManager
[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour {
    public static GameManager instance;

    [Header("Managers")]
    public InputManager inputManager;
    public PlayerJoinManager playerJoinManager;

    [Header("Important Scene Names")]
    public string mainMenuSceneName;

    private string currentSceneName;
    // Important Data from the Main Menu.
    // this is by default set to true for all values so that the players spawn as normal if we start in a level.
    private bool[] readyPlayers = { true, true, true, true };

    public PlayerData DefaultPlayerData;
    public PlayerController PlayerPrefab;
    // Important Data in any non Main Menu scene.
    public Player[] players { get; private set; }
    public Player leader;
    [Range(0,1)]
    [Tooltip("Amount experiance is scalled by per level over")]
    public float ExpPenaltyPerLvl = 0.75f;
    [Range(0, 1)]
    [Tooltip("Amount of bonus experiance per level under")]
    public float ExpBonusPerLvl = 0.25f;

    public int countdown = 3; //at the start of a round
    public GameObject countdownText; 

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoaded;
    }

    IEnumerator Countdown()
    {
        DisablePlayersMovement(countdown);
        countdownText.SetActive(true);
        for (int i = countdown; i > 0; i--)
        {
            countdownText.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.SetActive(false);
    }

    void DisablePlayersMovement(float duration)
    {
        for(int i = 0; i < players.Length; i++) {
            if (players[i] != null)
                players[i].DisablePlayer(duration);
        }
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
                TileManager.tileManager.StartGame();
                players = new Player[readyPlayers.Length];
                Queue<SpawnTile> spawnPoints = new Queue<SpawnTile>(TileManager.tileManager.tileMap.SpawnTiles);
                for (int i = 0; i < readyPlayers.Length; ++i) {
                    if (readyPlayers[i]) {
                        players[i] = new Player(i, DefaultPlayerData);
                        SpawnTile spawnTile = spawnPoints.Dequeue();
                        players[i].SetController(Instantiate<GameObject>(PlayerPrefab.gameObject, spawnTile.transform.position, Quaternion.identity).GetComponent<PlayerController>());
                        players[i].onHurt += ExpOnHurt;
                        players[i].onDeath += ExpOnKill;
                    }
                }
                FollowTargetsCamera moveCamera = Camera.main.GetComponentInParent<FollowTargetsCamera>();
                if (moveCamera != null) {
                    foreach (Player player in players) {
                        if (player != null) {
                            moveCamera.targets.Add(player.controller.gameObject);
                        }
                    }
                }
                // StartCoroutine(Countdown());
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
        float highestRank = 0;
        foreach (Player player in players) {
            if (player.rank > highestRank) {
                highestRank = player.rank;
                leader = player;
            } else if (player.rank == highestRank)
                leader = null;
        }
    }

    public void ExpOnHurt(Player damageDealer, Player reciever, float percentDealt) {
        if (damageDealer != null) {
            float experianceGain = percentDealt;
            if (damageDealer.rank > reciever.rank)
                experianceGain *= Mathf.Pow(ExpPenaltyPerLvl, damageDealer.rank - reciever.rank); // Scale Experiance gain down by amount overleveled
            else
                experianceGain *= 1 + ExpBonusPerLvl * (reciever.rank - damageDealer.rank); // Scale Experiance gain up according to amount underleveled
            StartCoroutine(ExperianceOverTime(damageDealer, experianceGain));
        }
    }

    public void ExpOnKill(Player killer, Player killed) {
        ExpOnHurt(killer, killed, 0.5f);
    }

    // Move to a position over a duration and slow down at the end
    private IEnumerator ExperianceOverTime(Player player, float amount) {
        float duration = Mathf.Sqrt(amount);
        float endTime = Time.time + duration;
        float remaining = amount;
        while (endTime > Time.time) {
            float add = remaining - amount * Mathf.Pow(Mathf.Max(endTime - Time.time, 0) / duration, 2f);
            player.AddExperiance(add);
            remaining -= add;
            yield return null;
        }
    }

    private class GameRound {

        public Dictionary<Player, float> initialExperiance;
        public Player[] players;

        public GameRound(Player[] players) {
            this.players = players;
            initialExperiance = new Dictionary<Player, float>();
            foreach (Player player in this.players) {
                initialExperiance.Add(player, player.experiance);
            }
        }
    }
}
