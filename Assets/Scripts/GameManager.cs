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
    [Tooltip("Amount of experiance per precent life dealt")]
    public float ExpGainPerDamage = 0.5f;
    [Tooltip("Amount of experiance for killing blow")]
    public float ExpGainOnKill = 0.25f;
    [Range(0,1)]
    [Tooltip("Amount experiance is scalled by per level over")]
    public float ExpPenaltyPerLvl = 0.75f;
    [Range(0, 1)]
    [Tooltip("Amount of bonus experiance per level under")]
    public float ExpBonusPerLvl = 0.25f;

    public int countdown = 3; //at the start of a round
    public GameObject countdownText;

    private List<GameRound> rounds = new List<GameRound>();

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
                if (players == null)
                    SetUpPlayers();
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
            rounds.Clear();
            players = null;
        }
        else {
            if (rounds.Count <= 0 || !rounds[rounds.Count - 1].isActive) {
                GameRound newRound = new GameRound(GetActivePlayers(players));
                rounds.Add(newRound);
                newRound.LoadLevel();
                StartCoroutine(StartGameAfterLoad(newRound));
            }
        }
    }

    public void SetUpPlayers() {
        players = new Player[readyPlayers.Length];
        for (int i = 0; i < readyPlayers.Length; ++i) {
            if (readyPlayers[i]) {
                players[i] = new Player(i, DefaultPlayerData);
                players[i].OnHurt += ExpOnHurt;
                players[i].OnDeath += ExpOnKill;
            }
        }
    }

    private IEnumerator StartGameAfterLoad(GameRound round) {
        while (!round.isReady)
            yield return null;
        round.StartGame();
        players[0].ResetSpecialMove(); 
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Returns an array containing only active players
    /// </summary>
    protected static Player[] GetActivePlayers(Player[] players) {
        List<Player> activePlayers = new List<Player>(players);
        for (int i = activePlayers.Count - 1; i >= 0; --i)
            if (activePlayers[i] == null)
                activePlayers.RemoveAt(i);
        return activePlayers.ToArray();
    }

    /// <summary>
    /// Update who the current highest ranking player is.
    /// </summary>
    private void UpdateRank() {
        float highestRank = 0;
        foreach (Player player in players) {
            if (player.rank > highestRank) {
                highestRank = player.rank;
                leader = player;
            }
            else if (player.rank == highestRank)
                leader = null;
        }
    }

    public void ExpOnHurt(Player damageDealer, Player reciever, float percentDealt) {
        if (damageDealer != null)
            StartCoroutine(ExperianceOverTime(damageDealer, ScaleExpGain(damageDealer.rank, reciever.rank, percentDealt * ExpGainPerDamage)));
    }

    public void ExpOnKill(Player killer, Player killed) {
        if (killer != null)
            StartCoroutine(ExperianceOverTime(killer, ScaleExpGain(killer.rank, killed.rank, ExpGainOnKill)));
    }

    private float ScaleExpGain(int dealerRank, int recieverRank, float amount) {
        float experianceGain = amount;
        if (dealerRank > recieverRank)
            experianceGain *= Mathf.Pow(ExpPenaltyPerLvl, dealerRank - recieverRank); // Scale Experiance gain down by amount overleveled
        else
            experianceGain *= 1 + ExpBonusPerLvl * (recieverRank - dealerRank); // Scale Experiance gain up according to amount underleveled
        return experianceGain;
    }

    /// <summary>
    /// Grant experiance to a player, This happens over time very quickly to give it that pokemon like level up
    /// </summary>
    private IEnumerator ExperianceOverTime(Player player, float amount) {
        float duration = Mathf.Sqrt(amount);
        float endTime = Time.time + duration;
        float remaining = amount;
        while (endTime > Time.time) {
            float add = remaining - amount * Mathf.Pow(Mathf.Max(endTime - Time.time, 0) / duration, 2f);
            player.AddExperiance(add);
            remaining -= add;
            UpdateRank(); // Update Ranking after granting experiance to a player
            yield return null;
        }
    }

    private class GameRound {

        public Dictionary<Player, float> initialExperiance;
        public Player[] players;
        public bool isActive {
            get { return activePlayersControllers.Count > 1 || isLoading || isReady; }
        }
        public bool isLoading { get; private set; }
        public bool isReady { get; private set; }
        public List<GameObject> activePlayersControllers = new List<GameObject>();

        public GameRound(Player[] players) {
            this.players = players;
            initialExperiance = new Dictionary<Player, float>();
            foreach (Player player in this.players) {
                initialExperiance.Add(player, player.experiance);
            }
            isLoading = isReady = false;
        }

        ~GameRound() {
            if (!isActive)
                GameOver();
        }

        public void LoadLevel() {
            isLoading = true;
            TileManager.tileManager.LoadLevel("LoadLevel").AddListener(delegate { isLoading = false; isReady = true; });
        }

        public void StartGame() {
            TileManager.tileManager.StartGame();

            Queue<SpawnTile> spawnPoints = new Queue<SpawnTile>(TileManager.tileManager.tileMap.SpawnTiles);
            FollowTargetsCamera moveCamera = Camera.main.GetComponentInParent<FollowTargetsCamera>();
            foreach (Player player in players) {
                player.ResetForRound();
                SpawnTile spawnTile = spawnPoints.Dequeue();
                player.SetController(Instantiate<GameObject>(GameManager.instance.PlayerPrefab.gameObject, spawnTile.transform.position, Quaternion.identity).GetComponent<PlayerController>());
                //player.ResetSpecialMove(); //this is to make sure that the SpecialMove has a reference to PlayerController, it can't be in the constructor
                moveCamera.targets.Add(player.controller.gameObject);
                activePlayersControllers.Add(player.controller.gameObject);
                player.OnDeath += Player_onDeath;
            }
            isReady = false;
        }

        private void GameOver() {
            foreach (Player player in players)
                player.ResetForRound();
            foreach (GameObject g in activePlayersControllers)
                Destroy(g);
            foreach (Player player in players)
                player.OnDeath -= Player_onDeath;
        }

        private void Player_onDeath(Player killer, Player killed) {
            activePlayersControllers.Remove(killed.controller.gameObject);
            if (!isActive)
                GameOver();
        }
    }
}
