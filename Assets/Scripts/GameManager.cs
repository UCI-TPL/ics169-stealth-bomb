﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

// Require an inputManager
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(WeatherManager))]
public class GameManager : MonoBehaviour {
    public static GameManager instance;


    [Header("Managers")]
    public InputManager inputManager;
    public PlayerJoinManager playerJoinManager;
    public AudioManager audioManager;
    public GameObject audioManagerPrefab;
    public WeatherManager weatherManager;

    public CurveManager curveManager; //used to reset the curves when a new round begins

    [Header("Important Scene Names")]
    public string mainMenuSceneName;
    public string winnerSceneName;
    private string currentSceneName;
    public LevelList[] levelNames;
    private string[] visited;
    private int pg_index;           // previous level group index.
    
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
    public int maxPoints {
        get {
            return maxRank * ExperianceSettings.PointsPerLevel;
        }
    }
    [Tooltip("All settings relating to experiance gained")]
    public ExperianceSettings ExperianceSettings;

    // variables for the game "timer"
    [Header("In-Game: 'Timer' Related")]
    public float TimeBeforeCrumble = 30f;
    public float TimeDecreasePerPlayer = 10f;

    [HideInInspector]
    public Vector3 GhostOffset = Vector3.zero; //when the map shrinks the ghosts will move closer 
    [HideInInspector]
    public float GhostOffsetLimit; //how far the ghosts are adjusted inwards 

    //public int PlayersKilled = 0;


    [Header("In-Game: Starting Countdown")]
    //public int countdown = 2; //at the start of a round
    public GameObject countdownText;

    private List<GameRound> rounds = new List<GameRound>();
    public bool inGame { get; private set; }
    public Transform PersistBetweenRounds { get; private set; }
    public readonly UnityEvent RoundReset = new UnityEvent();

    // keeps track of winners
    [HideInInspector]
    public List<Player> Winners = new List<Player>();

    [HideInInspector]
    public Dictionary<int,int> PlayerColor = new Dictionary<int,int>(); //meant to store player color, color index. This is set in the menu 
    [HideInInspector]
    private List<int> ColorIndexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7});
    //public List<int> PlayerColors = new List<int>(); //list of player colors that have been given out


    /* for Music: Finale
    whenever a player's x pis above full rank's 70%
    change battle music to finale
     */
     [Header("Audio related")]
     private static float finaleRank = 4f;
     private static string battle_music = "Battle";
     


    public void StartGame(bool[] playersReady, int[] xboxControllerNumbers) {
        // StopAllCoroutines();
        // string s = "Players Recieved from Main Menu: ";
        // for (int i = 0; i < playersReady.Length; ++i) {
        //   s += "player " + i.ToString() + ": " + playersReady[i].ToString() + "  ";
        // }
        // Debug.Log(s);
      
        rounds.Clear();
        SetUpPlayers(playersReady, xboxControllerNumbers);
        leader = null;
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
        Debug.Log("ReturnMenu() called.");
        instance.inGame = false;
        instance.Winners.Clear();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        instance.ColorIndexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }); //reset the available colors
        foreach (GameRound round in instance.rounds)
            round.EndGame();
        GameManager.instance.audioManager.Stop("Fanfare");
        GameManager.instance.audioManager.Stop(battle_music);
        GameManager.instance.audioManager.Play("Main Menu");
        SceneManager.LoadScene(instance.mainMenuSceneName);
    }

    public static void GoToWinScene() {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        GameManager.instance.audioManager.Stop("Fanfare");
        GameManager.instance.audioManager.Stop(battle_music);
        SceneManager.LoadScene(instance.winnerSceneName);
    }

    #region Audio
    //These are called by the Sliders in the UI
    public void Mute()
    {
        instance.audioManager.Mute();
    }
    public void SetMasterVolume(float volume)
    {
        instance.audioManager.SetMasterVolume(volume); // calling a function on audiomanager
    }

    public void SetMusicVolume(float volume)
    {
        instance.audioManager.SetMusicVolume(volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        instance.audioManager.SetSoundEffectVolume(volume);
    }

    #endregion
    // Use this for initialization
    void Awake () {
        //Application.targetFrameRate = 1500; //sets the frame rate. Make it like 1000 to run at max possible FPS
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoaded;
        battle_music = "Battle";
        
    }

    IEnumerator Countdown()
    {
        instance.audioManager.PlayRandomSound("Ready");

        if(countdownText != null) //only do the countdown if you can find it
        {
            DisablePlayersMovement(1f);
            Text cText = countdownText.GetComponent<Text>();
            countdownText.SetActive(true);
            switch(rounds[rounds.Count - 1].type) {
                case GameRound.RoundType.Normal:
                    cText.text = "READY";
                    break;
                case GameRound.RoundType.SuddenDeath:
                    cText.text = "Sudden Death!";
                    break;
            }
            yield return new WaitForSeconds(1f);
            cText.text = "GO!";
            yield return new WaitForSeconds(1f);
            countdownText.SetActive(false);
        }
        yield return null;            
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
                    SetUpPlayers(new bool[] { true, true, true, true }, new int[] { 0, 1, 2, 3 }); // Set up players if game is not started in main menu
                    StartCoroutine(UpdateGame());
                }
            }
            if (countdownText == null)
                countdownText = GameObject.FindGameObjectWithTag("countdown");
            if (audioManager == null)
            {
                audioManager = Instantiate(audioManagerPrefab).GetComponent<AudioManager>();
                audioManager.gameObject.name = "AudioManager"; //I don't like it being named "Clone"
                DontDestroyOnLoad(audioManager);
                if (scene.name == mainMenuSceneName)
                    GameManager.instance.audioManager.Play("Main Menu");
            }
            if(curveManager == null && scene.name != mainMenuSceneName && scene.name != winnerSceneName)
            {
                curveManager = GameObject.FindGameObjectWithTag("ghost-curve").GetComponent<CurveManager>();
                //This will be destroyed on load because a different scene might have a different curve manager
            }
        }
    }

    // Update exists here to handle some things are need to be done during a scene, not when a scene is loaded.
    IEnumerator UpdateGame()
    {
        inGame = true;
        PersistBetweenRounds = new GameObject("PersistBetweenRounds").transform;

        // Game update
        while (inGame) {
            if (rounds.Count <= 0 || !rounds[rounds.Count - 1].isActive) {
                CheckForWinner();
                if (Winners.Count == 1) {
                    GoToWinScene();
                    break;
                }
                else if (Winners.Count > 1) {
                    SuddenDeathRound suddenDeath = new SuddenDeathRound(Winners.ToArray(), rounds.Count);
                    rounds.Add(suddenDeath);
                    suddenDeath.LoadLevel();
                    StartCoroutine(StartGameAfterLoad(suddenDeath));
                    while (inGame) {
                        if (rounds.Count <= 0 || !rounds[rounds.Count - 1].isActive) {
                            GoToWinScene();
                            yield break;
                        }
                        yield return null;
                    }
                }

                GameRound newRound = new GameRound(GetActivePlayers(players), rounds.Count);
                rounds.Add(newRound);
                newRound.LoadLevel();
                StartCoroutine(StartGameAfterLoad(newRound));
            }
            // ReturnMenu();
            yield return null;
        }
    }

    public void SetUpPlayers(bool[] readyPlayers, int[] xboxControllerNumbers) {
        players = new Player[readyPlayers.Length];
        for (int i = 0; i < readyPlayers.Length; ++i) {
            if (readyPlayers[i]) {
                players[i] = new Player(i, xboxControllerNumbers[i], DefaultPlayerData);
            }
        }
    }

    public int AssignPlayerColor(int playerNum)
    {
        if(PlayerColor.ContainsKey(playerNum))
        {
            return PlayerColor[playerNum];
        }
        int randomIndex = UnityEngine.Random.Range(0, instance.ColorIndexes.Count);
        int colorIndex = instance.ColorIndexes[randomIndex];
        instance.ColorIndexes.Remove(colorIndex);
        PlayerColor[playerNum] = colorIndex;
        return colorIndex;
    }

    public int ExchangeColors(int playerNum) //for players to switch colors in the menu
    {
        if (!PlayerColor.ContainsKey(playerNum)) //if the player does not have a color just assign one and return that
            return AssignPlayerColor(playerNum);
        int randomIndex = UnityEngine.Random.Range(0, instance.ColorIndexes.Count);
        int colorIndex = instance.ColorIndexes[0];
        instance.ColorIndexes.Remove(colorIndex); //take a new color out for player
        instance.ColorIndexes.Add(PlayerColor[playerNum]); //return the current color to the list
        PlayerColor[playerNum] = colorIndex; //update dictionary with the new color
        return colorIndex;
    }

    public int GetPlayerColor(int playerNum)
    {
        if(PlayerColor.ContainsKey(playerNum)) //if the player has a color return that index, or get a new color and return that. 
            return PlayerColor[playerNum];
        else
            return AssignPlayerColor(playerNum);
    }
    
    public bool CanPause() //lets the Pause UI know if it is ok to pause, to prevent it from pausing when leaving the progress screeen or in any scene
    {
        int state = (int) instance.rounds[rounds.Count - 1].State; //converting enum to int
        return (state == 3 || state == 4) ? true : false; //return true if the game is in Battle or HurryUp
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
        if (leader != null && leader.rank >= finaleRank)
        {
            battle_music = "Finale";
        }
    }

    // helper method to check for winner
    private void CheckForWinner() {
        // ties are currently possible
        //Debug.Log("players null: " + (players == null));
        for (int i = 0; i < players.Length; i++) {
            if (players[i] != null && players[i].rank >= maxRank && !Winners.Contains(players[i])) {
                Winners.Add(players[i]);
                instance.audioManager.AnnounceWinner(i);
            }
        }
        //Debug.Log("number of winners in game manager = " + Winners.Count);
    }
    [Serializable]
    public class LevelList {
        public string[] LevelGroup;
    }

    public void AnnounceWinner(int pNumber) //players the "PLAYER WINS" audio
    {

    }

    public class GameRound {
        public RoundType type = RoundType.Normal;
        public readonly int RoundNumber;
        public Player[] players;
        private Dictionary<BonusExperiance.ExperianceType, Dictionary<Player, List<BonusExperiance>>> experianceGained;
        public Dictionary<BonusExperiance.ExperianceType, Dictionary<Player, List<BonusExperiance>>> ExperianceGained {
            get {
                if (experianceGained == null)
                    CalculateExperiance();
                return experianceGained;
            }
        }
        public bool isActive {
            get { return State != GameState.Finished && State != GameState.Created; }
        }
        public GameState State { get; protected set; }
        public float StartTime { get; private set; }
        public float ElapsedTime { get { return Time.time - StartTime; } }
        public float RoundEndTime = 0.5f; //time for the camera to focus on the surviving player before the round starts
        public List<GameObject> activePlayersControllers = new List<GameObject>();
        public List<GameObject> ghostPlayerControllers = new List<GameObject>();
        public int PlayersAlive { get { return activePlayersControllers.Count; } }
        private Scene roundScene;

        public Vector3 ghostSpawnLocation = new Vector3(13f,6f,13f); //this will have to be different per map! Will be changed later
        FollowTargetsCamera moveCamera;

        public bool roundEnding = false;

        private static string prevLevel = "";

        // A random number generator, used to pick a stage from an array in levelNames.
        System.Random rng = new System.Random();

        #region Player Stat Variables
        private readonly Player Leader;
        private Dictionary<Player, int> initialRank;
        public readonly Dictionary<Player, Dictionary<Player, float>> DamageDealt = new Dictionary<Player, Dictionary<Player, float>>();
        public readonly Dictionary<Player, Dictionary<Player, float>> DamageTaken = new Dictionary<Player, Dictionary<Player, float>>();
        public readonly Dictionary<Player, HashSet<Player>> Kills = new Dictionary<Player, HashSet<Player>>();
        public readonly Dictionary<Player, Player> KilledBy = new Dictionary<Player, Player>();
        #endregion


        public GameRound(Player[] players, int roundNumber) {
            this.players = players;
            RoundNumber = roundNumber;
            initialRank = new Dictionary<Player, int>();
            // Set Up Player Stats
            Leader = GameManager.instance.leader;
            foreach (Player player in this.players) {
                initialRank.Add(player, player.rank);
                DamageDealt.Add(player, new Dictionary<Player, float>());
                DamageTaken.Add(player, new Dictionary<Player, float>());
                Kills.Add(player, new HashSet<Player>());
                foreach (Player other in this.players) {
                    DamageDealt[player].Add(other, 0);
                    DamageTaken[player].Add(other, 0);
                }
            }

            State = GameState.Created;
        }

        public void EndGame() {
            if (isActive)
                Reset();
        }

        private static int mapCount = 0;

        // Make sure that the levels that are made have only the tile maps before being uploaded.
        public void LoadLevel() {
            State = GameState.Loading;

            //int groupNum = PickLevelGroup();
            //string[] chosenGroup = instance.levelNames[groupNum].LevelGroup;
            //TileManager.tileManager.LoadLevel(chosenGroup[mapCount++ % chosenGroup.Length], (Scene loadedScene) => { State = GameState.Ready; roundScene = loadedScene; });

            List<string> availableLevels = new List<string>();
            for (int i = Mathf.Min(1, GameManager.instance.rounds.Count / 2); i <= GameManager.instance.rounds.Count / 2; ++i) {
                availableLevels.AddRange(instance.levelNames[i].LevelGroup);
            }
            string ChosenLevel;
            do {
                ChosenLevel = availableLevels[UnityEngine.Random.Range(0, availableLevels.Count)];
            } while (ChosenLevel == prevLevel);
            prevLevel = ChosenLevel;
            TileManager.tileManager.LoadLevel(ChosenLevel, (Scene loadedScene) => { State = GameState.Ready; roundScene = loadedScene; });
            Resources.UnloadUnusedAssets();
        }

        // This returns an integer that corresponds to a level group that LoadLevel will load a level
        // from. 0 = level group 1, 1 = level group 2, and so on.
        private int PickLevelGroup() {
            // counters for the number of players who's ranks fall within a particular level group.
            int[] pr_Groups = { 0, 0, 0, 0 };

            // increment the counters based on the player ranks.
            int ranksPerGroup;
            if (GameManager.instance.maxRank >= 6)
                ranksPerGroup = Mathf.CeilToInt(GameManager.instance.maxRank / 4);
            else if (GameManager.instance.maxRank >= 4 && GameManager.instance.maxRank < 6)
                ranksPerGroup = Mathf.FloorToInt(GameManager.instance.maxRank / 4);
            else
                return 3; // return the last level group if playing with few ranks.

            foreach (Player p in players)
            {
                if (p.rank < ranksPerGroup * 1)
                    pr_Groups[0]++;
                else if (p.rank >= ranksPerGroup * 1 && p.rank < ranksPerGroup * 2)
                    pr_Groups[1]++;
                else if (p.rank >= ranksPerGroup * 2 && p.rank < ranksPerGroup * 3)
                    pr_Groups[2]++;
                else
                    pr_Groups[3]++;
            }

            /*
            int pr_GroupMax = pr_Groups.Max();
            int chosenGroup = Array.IndexOf(pr_Groups, pr_GroupMax);
            return chosenGroup;  
            */

            int index = 0;
            int maxValue = 0;
            for (int i = 0; i < pr_Groups.Length; i++)
            {
                if (pr_Groups[i] > maxValue)
                {
                    index = i;
                    maxValue = pr_Groups[i];
                }
                else if (pr_Groups[i] == maxValue)
                {
                    index = i;
                }
            }
            return index;
        }

        public void ResetCurves() //must happen every round just in case the map changes 
        {
            MapInfo newMap = GameObject.Find("Tile Map").GetComponent<MapInfo>(); //this has the info for the Ghost Curve for each map 
            GameManager.instance.curveManager.ResetCurve1(newMap.LeftCurve, (int) newMap.min, (int)newMap.max);
            GameManager.instance.curveManager.ResetCurve2(newMap.RightCurve, (int)newMap.min, (int)newMap.max);

            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
            foreach (GameObject g in ghosts)
            {
                Destroy(g);
            }
        }

        public void StartGame() {
            GameManager.instance.GhostOffset = Vector3.zero; 
            GameManager.instance.UpdateRank();
            GameManager.instance.audioManager.Stop("Fanfare");
            GameManager.instance.audioManager.Stop("Main Menu");
            GameManager.instance.audioManager.Play(battle_music);

            ResetCurves();

            string s = "Starting round with players: ";
            foreach (Player player in players)
                s += player.playerNumber.ToString() + ", ";
            //Debug.Log(s);

            //Debug.Log("This is where the round indeed starts amirighte?");
            SceneManager.SetActiveScene(roundScene);
            StartTime = Time.time;
            GameManager.instance.weatherManager.ChangeWeather(1);
            TileManager.tileManager.StartGame();

            ItemSpawner.Instance.UpdateSpawnPoints(TileManager.tileManager.tileMap);
            Queue<SpawnTile> spawnPoints = new Queue<SpawnTile>(TileManager.tileManager.tileMap.SpawnTiles);
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
                {
                    foreach (GameObject g in ghostPlayerControllers)
                    {
                        if(g.GetComponent<PlayerController>().gameObject)
                            moveCamera.targets.Remove(g.GetComponent<PlayerController>().gameObject);
                    }
                    activePlayersControllers.FirstOrDefault().GetComponent<PlayerController>().invinsible = true;
                    yield return new WaitForSeconds(RoundEndTime);
                    
                    GameOver();
                }
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

        /// <summary>
        /// Invoked when a round ends. Should save all events that happened during the round and prep for the next round
        /// </summary>
        protected virtual void GameOver() {
            // Recalculate Experiance Gained
            foreach (KeyValuePair<Player, int> exp in CalculateExperiance())
                exp.Key.AddExperiance(exp.Value);

            GameManager.instance.audioManager.Stop(battle_music);
            GameManager.instance.audioManager.Play("Fanfare");
            State = GameState.ProgressScreen;
            // bool gameWon = false;
            // // check to see if any players won the game
            // for (int i = 0; i < players.Length; i++) {
            //     if (players[i].rank >= GameManager.instance.maxRank) {
            //         gameWon = true;
            //         break;
            //     }
            // }
            
            ProgressScreenUI.Instance.StartProgressScreen(this, Reset);

        }

        protected void Reset() {
            foreach (GameObject g in activePlayersControllers)
                g.GetComponent<PlayerController>().Destroy();
            foreach (GameObject g in ghostPlayerControllers)
            {
                g.GetComponent<GhostController>().Destroy();
            }
            foreach (Player player in players) {
                player.OnHurt -= Player_onHurt;
                player.OnDeath -= Player_onDeath;
            }
            State = GameState.Finished;
            GameManager.instance.RoundReset.Invoke();
        }

        private void Player_onHurt(Player damageDealer, Player reciever, float percentDealt) {
            // Update Player Stats
            DamageDealt[damageDealer][reciever] += percentDealt;
            DamageTaken[reciever][damageDealer] += percentDealt;
        }

        private void Player_onDeath(Player killer, Player killed) {

            moveCamera.targets.Remove(killed.controller.gameObject);             // Stop camera from tracking dead player

            instance.audioManager.PlayRandomSound("Death");

            activePlayersControllers.Remove(killed.controller.gameObject);
            Vector3 deathPosition = killed.controller.transform.position;
            //if (deathPosition.y < 0)
            //deathPosition = new Vector3(deathPosition.x, 10f, deathPosition.z); // Why is this hardcoded??? moved it to ghost controller line 73 because i want the ghost to spawn at the player then move
            instance.StartCoroutine(InstantiateGhost(killed.playerNumber, killed, deathPosition));
            killed.controller.DisableUI();
            // Update Player Stats
            if (killer != null)
                Kills[killer].Add(killed);
            KilledBy.Add(killed, killer);
        }

        private void Ghost_onDeath(Player killer, Player killed)
        {
            ghostPlayerControllers.Remove(killed.controller.gameObject);
        }

        public IEnumerator InstantiateGhost(int killedNum, Player killed, Vector3 deathPosition)
        {
            if(activePlayersControllers.Count > 1) //Don't spawn the last player as a ghost
            {
                yield return new WaitForSeconds(0.5f);
                if (isActive && !killed.ghost) { //this means that the killed player has already been made into a ghost 

                    players[killedNum].ResetHealth();
                    GameObject tempGhost = Instantiate<GameObject>(GameManager.instance.GhostPrefab.gameObject, deathPosition, Quaternion.identity);
                    if(tempGhost)
                    {
                        players[killedNum].SetGhost(tempGhost.GetComponent<PlayerController>()); //SetGhost works like SetController but without weapons
                        ghostPlayerControllers.Add(players[killedNum].controller.gameObject); //to make sure it gets deleted
                        moveCamera.targets.Add(players[killedNum].controller.gameObject);
                    }
                   

                }
            }
        }

        private Dictionary<Player, int> CalculateExperiance() {
            // Running total of exp gained used for comeback calculation
            Dictionary<Player, int> totalExp = new Dictionary<Player, int>();
            foreach (Player player in players)
                totalExp.Add(player, 0); // Initialize total exp
            // Resulting exp gained dict
            experianceGained = new Dictionary<BonusExperiance.ExperianceType, Dictionary<Player, List<BonusExperiance>>>();
            // Calculate each exp type in specified order
            foreach (BonusExperiance.ExperianceType type in GameManager.instance.ExperianceSettings.ExperianceOrder) {
                Dictionary<Player, List<BonusExperiance>> expForType = CalculateExperiance(type);
                if (expForType != null) {
                    foreach (KeyValuePair<Player, List<BonusExperiance>> keyValuePair in expForType)
                        foreach (BonusExperiance exp in keyValuePair.Value)
                            totalExp[keyValuePair.Key] += exp.Points;
                    experianceGained.Add(type, expForType);
                }
            }
            return totalExp;

            Dictionary<Player, List<BonusExperiance>> CalculateExperiance(BonusExperiance.ExperianceType experianceType) {
                Dictionary<Player, List<BonusExperiance>> playerExperiance = new Dictionary<Player, List<BonusExperiance>>();
                foreach (Player player in players) {
                    List<BonusExperiance> playerExpGained = new List<BonusExperiance>();

                    // Depending on type of experiance run different calculations
                    switch (experianceType) {
                        case BonusExperiance.ExperianceType.Damage:
                            float totalDamage = 0;
                            foreach (float damage in DamageDealt[player].Values)
                                totalDamage += damage;
                            BonusExperiance damageExp = GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.Damage).MulPoints(totalDamage * 2);
                            if (damageExp.Points > 0)
                                playerExpGained.Add(damageExp);
                            break;
                        case BonusExperiance.ExperianceType.Kill:
                            foreach (Player p in Kills[player])
                                playerExpGained.Add(GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.Kill));
                            break;
                        case BonusExperiance.ExperianceType.LastOneStanding:
                            if (!KilledBy.ContainsKey(player))
                                playerExpGained.Add(GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.LastOneStanding));
                            break;
                        case BonusExperiance.ExperianceType.KillLeader:
                            if (Kills[player].Contains(Leader))
                                playerExpGained.Add(GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.KillLeader));
                            break;
                        case BonusExperiance.ExperianceType.Revenge:
                            if (RoundNumber > 0 && GameManager.instance.rounds[RoundNumber - 1].KilledBy.ContainsKey(player) && Kills[player].Contains(GameManager.instance.rounds[RoundNumber - 1].KilledBy[player]))
                                playerExpGained.Add(GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.Revenge));
                            break;
                        case BonusExperiance.ExperianceType.NoDamageTaken:
                            if (KilledBy.ContainsKey(player))
                                break;
                            float damagetaken = 0;
                            foreach (float damage in DamageTaken[player].Values)
                                if ((damagetaken += damage) > 0)
                                    break;

                            if (damagetaken == 0)
                                playerExpGained.Add(GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.NoDamageTaken));
                            break;
                        case BonusExperiance.ExperianceType.Comeback:
                            if (Leader != null && initialRank[player] <= Leader.rank - 2) {
                                BonusExperiance c = GameManager.instance.ExperianceSettings.GetExperiance(BonusExperiance.ExperianceType.Comeback).AddPoints(Mathf.FloorToInt(totalExp[player] * 0.5f));
                                if (c.Points > 0)
                                    playerExpGained.Add(c);
                            }
                            break;
                    }

                    if (playerExpGained.Count > 0) // Add player experiance if not empty
                        playerExperiance.Add(player, playerExpGained);
                }
                return playerExperiance.Count > 0 ? playerExperiance : null; // Return null if no experiance gained
            }
        }

        /// <summary>
        /// Instance of experiance gained during a round, Specifies how to display a set of experiance gained
        /// </summary>
        [System.Serializable]
        public struct BonusExperiance {
            public string Name;
            public Color Color;
            public int Points;
            public Sprite Sprite;

            public BonusExperiance AddPoints(int amount) {
                Points += amount;
                return this;
            }

            public BonusExperiance MulPoints(float amount) {
                Points = Mathf.FloorToInt(Points * amount);
                return this;
            }

            public enum ExperianceType {
                Damage, Kill, LastOneStanding, KillLeader, Revenge, NoDamageTaken, Comeback
            }
        }

        public enum GameState {
            Created, Loading, Ready, Battle, HurryUp, ProgressScreen, Finished
        }

        public enum RoundType { Normal, SuddenDeath }
    }

    public class SuddenDeathRound : GameRound {
        public SuddenDeathRound(Player[] players, int roundNumber) : base(players, roundNumber) {
            type = RoundType.SuddenDeath;
        }

        protected override void GameOver() {
            GameManager.instance.audioManager.Stop(battle_music);
            GameManager.instance.audioManager.Play("Fanfare");
            GameManager.instance.Winners = new List<Player> { activePlayersControllers.FirstOrDefault().GetComponent<PlayerController>().player };
            Reset();
        }
    }
}
