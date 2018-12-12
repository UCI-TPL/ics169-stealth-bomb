using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

// Require an inputManager
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour {
    public static GameManager instance;

    [Header("Managers")]
    public InputManager inputManager;
    public PlayerJoinManager playerJoinManager;
    public AudioManager audioManager;

    [Header("Important Scene Names")]
    public string mainMenuSceneName;
    private string currentSceneName;
    
    // Important Data from the Main Menu.
    // this is by default set to true for all values so that the players spawn as normal if we start in a level.
    //private bool[] readyPlayers = { true, true, true, true };

    [Header("In-game: Player Related")]
    public PlayerData DefaultPlayerData;
    public PlayerController PlayerPrefab;
    public PlayerController GhostPrefab;
    //public PlayerController GhostPrefab;
    // Important Data in any non Main Menu scene.
    public Player[] players { get; private set; }
    public Player leader;
    [Tooltip("Ranks required to win")]
    public int maxRank = 10;
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

    // variables for the game "timer"
    [Header("In-Game: 'Timer' Related")]
    public float TimeBeforeCrumble = 30f;
    public float TimeDecreasePerPlayer = 10f;
    
    //public int PlayersKilled = 0;

    [Header("In-Game: Starting Countdown")]
    public int countdown = 3; //at the start of a round
    public GameObject countdownText;

    private List<GameRound> rounds = new List<GameRound>();
    private bool inGame = false;
    public Transform PersistBetweenRounds { get; private set; }
    public readonly UnityEvent RoundReset = new UnityEvent();

    public void StartGame(bool[] playersReady) {
        string s = "Players Recieved from Main Menu: ";
        for (int i = 0; i < playersReady.Length; ++i) {
          s += "player " + i.ToString() + ": " + playersReady[i].ToString() + "  ";
        }
        Debug.Log(s);
        rounds.Clear();
        SetUpPlayers(playersReady);
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().buildIndex + 1, delegate { StartCoroutine(UpdateGame()); }));
    }

    // Loads scene and starts game once finished
    private IEnumerator LoadLevelAsync(int sceneBuildIndex, UnityAction OnFinishLoad) {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneBuildIndex);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }
        if (OnFinishLoad != null)
            OnFinishLoad.Invoke();
    }

    // Loads scene and starts game once finished
    private IEnumerator LoadLevelAsync(string sceneName, UnityAction OnFinishLoad) {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoadLevel.isDone) {
            yield return null;
        }
        if (OnFinishLoad != null)
            OnFinishLoad.Invoke();
    }

    public static void ReturnMenu() {
        instance.inGame = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        foreach (GameRound round in instance.rounds)
            round.EndGame();
        SceneManager.LoadScene(instance.mainMenuSceneName);
    }

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
        float startTime = Time.time;
        DisablePlayersMovement(countdown);
        countdownText.SetActive(true);
        float timeRemaining;
        while (inGame && (timeRemaining = startTime + countdown - Time.time) > 0) {
            countdownText.GetComponent<Text>().text = Mathf.Ceil(timeRemaining).ToString();
            yield return null;
        }
        if (countdownText != null) // Ensure that game scene is still active, its possible that player returned to main menu
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
                if (players == null) {
                    Debug.LogWarning("GameManager did not recieve players from main menu, defaulting to 4 players on, This is correct if starting editor from game scene");
                    SetUpPlayers(new bool[] { true, true, true, true }); // Set up players if game is not started in main menu
                    StartCoroutine(UpdateGame());
                }
            }
            if (countdownText == null)
                countdownText = GameObject.FindGameObjectWithTag("countdown");
        }
    }

    // Update exists here to handle some things are need to be done during a scene, not when a scene is loaded.
    IEnumerator UpdateGame()
    {
        inGame = true;
        PersistBetweenRounds = new GameObject("PersistBetweenRounds").transform;

        // Game update
        while (inGame) {
            if (rounds.Count <= 0 || !rounds[rounds.Count - 1].isActive) { // Check if there are no rounds or if the last round is not active
                GameRound newRound = new GameRound(GetActivePlayers(players)); // Create a new round
                rounds.Add(newRound);
                newRound.LoadLevel(); // Set up round
                StartCoroutine(StartGameAfterLoad(newRound)); // Start round once setup is complete
            }
            yield return null;
        }
    }

    public void SetUpPlayers(bool[] readyPlayers) {
        players = new Player[readyPlayers.Length];
        for (int i = 0; i < readyPlayers.Length; ++i) {
            if (readyPlayers[i]) {
                players[i] = new Player(i, DefaultPlayerData);
            }
        }
    }

    private IEnumerator StartGameAfterLoad(GameRound round) {
        while (round.State != GameRound.GameState.Ready)
            yield return null;
        round.StartGame();
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
    /// Update who the current highest experiance player is.
    /// </summary>
    public void UpdateRank() {
        float highestEXP = 0;
        foreach (Player player in GetActivePlayers(players)) {
            if (player.experiance > highestEXP) {
                highestEXP = player.experiance;
                leader = player;
            }
            else if (player.experiance == highestEXP)
                leader = null;
        }
    }

    public float ExpOnHurt(Player damageDealer, Player reciever, float percentDealt) {
        if (damageDealer != null)
            return damageDealer.AddExperiance(ScaleExpGain(damageDealer.rank, reciever.rank, percentDealt * ExpGainPerDamage));
            //StartCoroutine(ExperianceOverTime(damageDealer, ScaleExpGain(damageDealer.rank, reciever.rank, percentDealt * ExpGainPerDamage)));
        return 0;
    }

    public float ExpOnKill(Player killer, Player killed) {
        if (killer != null)
            return killer.AddExperiance(ScaleExpGain(killer.rank, killed.rank, ExpGainOnKill));
            //StartCoroutine(ExperianceOverTime(killer, ScaleExpGain(killer.rank, killed.rank, ExpGainOnKill)));
        return 0;
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
            //UpdateRank(); // Update Ranking after granting experiance to a player
            yield return null;
        }
    }

    public class GameRound {

        public Dictionary<Player, float> initialExperiance;
        public Player[] players;
        public bool isActive {
            get { return State != GameState.Finished && State != GameState.Created; }
        }
        public GameState State { get; private set; }
        public float StartTime { get; private set; }
        public float ElapsedTime { get { return Time.time - StartTime; } }
        public List<GameObject> activePlayersControllers = new List<GameObject>();
        public List<GameObject> ghostPlayerControllers = new List<GameObject>();
        public int PlayersAlive { get { return activePlayersControllers.Count; } }
        private Scene roundScene;

        public Vector3 ghostSpawnLocation = new Vector3(13f,6f,13f); //this will have to be different per map! Will be changed later

        FollowTargetsCamera moveCamera; 

        public bool roundEnding = false;

        public GameRound(Player[] players) {
            this.players = players;
            initialExperiance = new Dictionary<Player, float>();
            foreach (Player player in this.players) {
                initialExperiance.Add(player, player.experiance);
            }
            State = GameState.Created;
        }

        public void EndGame() {
            if (isActive)
                Reset();
        }

        public void LoadLevel() {
            State = GameState.Loading;
            TileManager.tileManager.LoadLevel("LoadLevel", (Scene loadedScene) => { State = GameState.Ready; roundScene = loadedScene; });
            Resources.UnloadUnusedAssets();
        }

        public void StartGame() {
            GameManager.instance.UpdateRank();
            string s = "Starting round with players: ";
            foreach (Player player in players)
                s += player.playerNumber.ToString() + ", ";
            Debug.Log(s);

            SceneManager.SetActiveScene(roundScene);
            StartTime = Time.time;
            TileManager.tileManager.StartGame();

            ItemSpawner.Instance.UpdateSpawnPoints(TileManager.tileManager.tileMap);
            Queue<SpawnTile> spawnPoints = new Queue<SpawnTile>(TileManager.tileManager.tileMap.SpawnTiles);
            //FollowTargetsCamera moveCamera = Camera.main.GetComponentInParent<FollowTargetsCamera>();
            moveCamera = Camera.main.GetComponentInParent<FollowTargetsCamera>();
            foreach (Player player in players) {
                player.ResetForRound();
                SpawnTile spawnTile = spawnPoints.Dequeue();
                player.SetController(Instantiate<GameObject>(GameManager.instance.PlayerPrefab.gameObject, spawnTile.transform.position, Quaternion.identity).GetComponent<PlayerController>());
                moveCamera.targets.Add(player.controller.gameObject);
                activePlayersControllers.Add(player.controller.gameObject);
                player.OnHurt += Player_onHurt;
                player.OnDeath += Player_onDeath;
            }
            State = GameState.Battle;
            GameManager.instance.StartCoroutine(Update());
        }

        public IEnumerator Update() {
            while (State == GameState.Battle || State == GameState.HurryUp) {
                if (activePlayersControllers.Count < 2)
                    GameOver();
                switch (State) {
                    case GameState.Battle:
                        if (ElapsedTime > GameManager.instance.TimeBeforeCrumble - (GameManager.instance.TimeDecreasePerPlayer * (players.Length - PlayersAlive))) {
                            TileManager.tileManager.StartCountdown();
                            State = GameState.HurryUp;
                        }
                        break;
                }
                yield return null;
            }
        }

        private void GameOver() {
            State = GameState.ProgressScreen;
            ProgressScreenUI.Instance.StartProgressScreen(this, Reset);
        }

        private void Reset() {
            foreach (GameObject g in activePlayersControllers)
                g.GetComponent<PlayerController>().Destroy();
            foreach (GameObject g in ghostPlayerControllers)
            {
                g.GetComponent<PlayerController>().Destroy();
            }
            foreach (Player player in players) {
                player.OnHurt -= Player_onHurt;
                player.OnDeath -= Player_onDeath;
            }
            State = GameState.Finished;
            GameManager.instance.RoundReset.Invoke();
        }

        private void Player_onHurt(Player damageDealer, Player reciever, float percentDealt) {
            GameManager.instance.ExpOnHurt(damageDealer, reciever, percentDealt);
        }

        private void Player_onDeath(Player killer, Player killed) {
            GameManager.instance.ExpOnKill(killer, killed);

            activePlayersControllers.Remove(killed.controller.gameObject);
            Vector3 deathPosition = killed.controller.transform.position;
            if (deathPosition.y < 6.5)
                deathPosition = ghostSpawnLocation; //will have to be changed per map
                //return;
            instance.StartCoroutine(InstantiateGhost(killed.playerNumber, killed, deathPosition));   
        }

        private void Ghost_onDeath(Player killer, Player killed)
        {
            ghostPlayerControllers.Remove(killed.controller.gameObject);
        }

        public IEnumerator InstantiateGhost(int killedNum, Player killed, Vector3 deathPosition)
        {
            if(activePlayersControllers.Count  > 1) //Don't spawn the last player as a ghost
            {
                yield return new WaitForSeconds(0.3f);
                players[killedNum].ResetHealth();
                players[killedNum].SetGhost(Instantiate<GameObject>(GameManager.instance.GhostPrefab.gameObject, deathPosition, Quaternion.identity).GetComponent<PlayerController>()); //SetGhost works like SetController but without weapons
                ghostPlayerControllers.Add(players[killedNum].controller.gameObject); //to make sure it gets deleted
                moveCamera.targets.Add(players[killedNum].controller.gameObject);
            }
        }

        /// <summary>
        /// Instance of experiance gained during a round, Specifies how to display a set of experiance gained
        /// </summary>
        public class BonusExperiance {
            public virtual string Name { get; private set; }
            public readonly Color Color;
            public float Experiance { get; private set; }

            public BonusExperiance(string name, Color color) {
                Name = name;
                Color = color;
            }

            public float AddExperiance(float amount) {
                return Experiance += amount;
            }
        }

        public enum GameState {
            Created, Loading, Ready, Battle, HurryUp, ProgressScreen, Finished
        }
    }
}
