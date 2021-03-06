﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressScreenUI : MonoBehaviour {
    
    private static ProgressScreenUI instance;
    public static ProgressScreenUI Instance {
        get {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<ProgressScreenUI>();
            if (instance == null)
                Debug.LogError("ProgressScreenUI not found, Please add ProgressScreenUI prefab to game scene");
            return instance;
        }
    }
    private Canvas canvas;

    [SerializeField]
    private RectTransform ProgressScreenRect;

    //[SerializeField]
    //private RectTransform PlayerIconsRect;
    //[SerializeField]
    //private GameObject PlayerIconPrefab;
    //[SerializeField]
    //private RectTransform ProgressScreenSliderRect;
    //[SerializeField]
    //private GameObject ProgressScreenSliderPrefab;
    [SerializeField]
    private RectTransform PlayerEXPRect;
    [SerializeField]
    private GameObject PlayerEXPBarPrefab;

    [Header("Level Settings")]
    [SerializeField]
    private RectTransform LevelTextRect;
    [SerializeField]
    private GameObject LevelTextPrefab;
    [SerializeField]
    private RectTransform LevelThresholdRect;
    [SerializeField]
    private GameObject DottedLinePrefab;
    //[SerializeField]
    //private RankRulerUI rankRulerUI;
    //[SerializeField]
    //private LayoutElement RulerTextLayoutElement;

    private Dictionary<Player, PlayerProgressObject> PlayerUIs;
    private Player[] players;

    [HideInInspector]
    public bool GameWon { get; private set; }

    [SerializeField]
    private GameObject pointText;

    [SerializeField]
    private CanvasGroup SkipProgressText;
    [SerializeField]
    private CanvasGroup ContinueText;
    private float DisplaySpeedScale = 1;

    //[SerializeField]
    //private UnityEvent Damage;
    //[SerializeField]
    //private UnityEvent Kill;
    //[SerializeField]
    //private UnityEvent LastOneStanding;
    //[SerializeField]
    //private UnityEvent KillLeader;
    //[SerializeField]
    //private UnityEvent Revenge;
    //[SerializeField]
    //private UnityEvent NoDamageTaken;
    //[SerializeField]
    //private UnityEvent ComeBack;

    // Use this for initialization
    void Awake () {
        instance = this;
        canvas = GetComponentInParent<Canvas>();
        GameWon = false;
    }

    private void Update() {
        // Only run updates if active
        if (ProgressScreenRect.gameObject.activeInHierarchy) {
            //// Fix ruler layout
            //RulerTextLayoutElement.minWidth = RulerTextLayoutElement.preferredWidth = PlayerIconsRect.rect.width;

            //UpdateCrown();
        }
    }

    /// <summary>
    /// Displays the progress screen overlay, and pauses game.
    /// </summary>
    /// <param name="round"> Gameround the progress screen is to display </param>
    /// <param name="action"> Event thats invoked when the progress screen is closed. </param>
    public void StartProgressScreen(GameManager.GameRound round, UnityAction action = null) {
        players = round.players; // Save list of players
        if (PlayerUIs == null)
            SetUpProgressScreen();
        DisplayScreen(1f);
        StartCoroutine(SlowPause(2, 0.05f));
        // Set up progress screen if it hasnt yet been setup

        // players should only be able to reach rank 10 once per game before being forced to head back to main menu.
        //RunEndgameChecksAndSetup();
        SkipProgressText.alpha = 0;
        ContinueText.alpha = 0;
        foreach (PointText p in ProgressScreenRect.GetComponentsInChildren<PointText>())
            Destroy(p?.gameObject);
        DisplaySpeedScale = 1;
        InvokeUnscaled(delegate {
            StartCoroutine(AddExperiance(round, action));
        }, 1.5f);
        InvokeUnscaled(delegate {
            StartCoroutine(ChangeAlpha(SkipProgressText, 1, 0.25f));
            StartCoroutine(InvokeOnPressStart(round.players, delegate { SpeedUpProgressScreen(); }));
        }, 1f);

        //// Setup the closing the progress screen by pressing start
        //StartCoroutine(InvokeOnPressStart(round.players, delegate{ PressStart(action); }));
    }

    private void SpeedUpProgressScreen() {
        DisplaySpeedScale = 5;
        foreach (PointText p in ProgressScreenRect.GetComponentsInChildren<PointText>())
            p.SetSpeed(DisplaySpeedScale);
        StartCoroutine(ChangeAlpha(SkipProgressText, -1, 0.25f));
    }

    private void PressStart(UnityAction action) {
        StopAllCoroutines(); // Ensure all coroutines and invokes are reset
        CancelInvoke();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        action.Invoke();
        ProgressScreenRect.gameObject.SetActive(false);
    }

    private IEnumerator AddExperiance(GameManager.GameRound round, UnityAction action) {
        float testTime = Time.unscaledTime;
        foreach (GameManager.GameRound.BonusExperiance.ExperianceType expType in GameManager.instance.ExperianceSettings.ExperianceOrder) {
            if (round.ExperianceGained.ContainsKey(expType)) {
                PointText p = Instantiate<GameObject>(pointText, ProgressScreenRect).GetComponent<PointText>();
                p.SetText(GameManager.instance.ExperianceSettings.GetExperiance(expType).Name);
                p.SetSpeed(DisplaySpeedScale);
                yield return new WaitForSecondsRealtime(0.5f / DisplaySpeedScale);
                int biggestStack = 0;
                foreach (KeyValuePair<Player, List<GameManager.GameRound.BonusExperiance>> keyValuePair in round.ExperianceGained[expType]) {
                    biggestStack = Mathf.Max(biggestStack, keyValuePair.Value.Count);
                    foreach (GameManager.GameRound.BonusExperiance exp in keyValuePair.Value) {
                        PlayerUIs[keyValuePair.Key].expBar.AddPoints(exp, 0.5f / DisplaySpeedScale);
                    }
                }


                yield return new WaitForSecondsRealtime(0.5f / DisplaySpeedScale);
                //Debug.Log(testTime - Time.unscaledTime);
            }
        }
        StartCoroutine(ChangeAlpha(SkipProgressText, -1, 0.25f));
        StartCoroutine(ChangeAlpha(ContinueText, 1, 0.5f));
        // Setup the closing the progress screen by pressing start
        StartCoroutine(InvokeOnPressStart(round.players, delegate { PressStart(action); }));
    }

    private IEnumerator ChangeAlpha(CanvasGroup canvasGroup, float value, float duration) {
        float endTime = duration + Time.unscaledTime;
        while (endTime > Time.unscaledTime) {
            canvasGroup.alpha += value / duration * Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha += value;
    }

    /// <summary>
    /// Grant experiance to a player, This happens over time very quickly to give it that pokemon like level up
    /// </summary>
    private IEnumerator ExperianceOverTime(Slider expSlider, float target) {
        float duration = Mathf.Sqrt(target - expSlider.value);
        float endTime = Time.unscaledTime + duration;
        float distance = expSlider.value - target;
        while (endTime > Time.unscaledTime) {
            expSlider.value = target + distance * Mathf.Pow((endTime - Time.unscaledTime) / duration, 2f);
            yield return null;
        }
        expSlider.value = target;
    }

    /// <summary>
    /// Update which player has the crown.
    /// </summary>
    //public void UpdateCrown() {
    //    float highestEXP = 0;
    //    PlayerProgressObject leader = null;
    //    foreach (PlayerProgressObject player in PlayerUIs) {
    //        player.expBar.IsLeader = false;
    //        if (player.Experiance > highestEXP) {
    //            highestEXP = player.Experiance;
    //            leader = player;
    //        }
    //        else if (player.Experiance == highestEXP)
    //            leader = null;
    //    }
    //    if (leader != null)
    //        leader.expBar.IsLeader = true;
    //}

    /// <summary>
    /// Invokes the event after any of the listed players press start
    /// </summary>
    /// <param name="players"> List of players to listen to </param>
    /// <param name="unityEvent"> Event to be invoked </param>
    private IEnumerator InvokeOnPressStart(Player[] players, UnityAction action) {
        bool pressedStart = false;
        UnityAction checkStart = delegate { pressedStart = true; };
        foreach (Player player in players)
            InputManager.inputManager.controllers[player.inputControllerNumber].start.OnDown.AddListener(checkStart);
        while (!pressedStart)
            yield return null;
        foreach (Player player in players)
            InputManager.inputManager.controllers[player.inputControllerNumber].start.OnDown.RemoveListener(checkStart);
        action.Invoke();
    }

    /// <summary>
    /// Sets the progress screen to active and moves the screen into view from the bottom of the screen
    /// </summary>
    private void DisplayScreen(float animDuration) {
        ProgressScreenRect.gameObject.SetActive(true);
        ProgressScreenRect.anchoredPosition = Vector2.zero; // forces the screen's anchored position to zero, this is the default position
        Vector3 targetPosition = ProgressScreenRect.anchoredPosition; // saves the default position as the target position
        ProgressScreenRect.anchoredPosition = new Vector2(0, -canvas.GetComponent<RectTransform>().rect.height); // Places the screen just under the screen out of view
        StartCoroutine(MoveSmooth(ProgressScreenRect, targetPosition, animDuration));
    }

    private void SetUpProgressScreen() {
        PlayerUIs = new Dictionary<Player, PlayerProgressObject>();
        for (int i = 0; i < players.Length; ++i) {
            PlayerProgressObject playerProgressObject;
            if (PlayerUIs.Count <= i) { // If there are not enough already created Player UIs then make a new one
                GameObject playerEXPBar = Instantiate<GameObject>(PlayerEXPBarPrefab, PlayerEXPRect);
                playerProgressObject = new PlayerProgressObject(playerEXPBar, GameManager.instance.maxPoints);
                PlayerUIs.Add(players[i], playerProgressObject);
            }
            PlayerUIs[players[i]].GO_progressBar.GetComponent<ProgressScreen_EXPBar>().SetColor(players[i].Color);
        }

        // Set up text display of number of levels to win
        Instantiate<GameObject>(LevelTextPrefab, LevelTextRect).GetComponent<Text>().text = "Win!";
        Instantiate<GameObject>(DottedLinePrefab, LevelThresholdRect);
        for (int i = GameManager.instance.maxRank-1; i >= 1; --i) {
            Instantiate<GameObject>(LevelTextPrefab, LevelTextRect).GetComponent<Text>().text = i.ToString();
            Instantiate<GameObject>(DottedLinePrefab, LevelThresholdRect);
        }

        //rankRulerUI.SetRange(0, GameManager.instance.maxRank); // Setup ruler to display number of ranks
    }

    // Move to a position over a duration and slowing down near end
    private IEnumerator MoveSmooth(RectTransform rt, Vector2 targetPosition, float duration) {
        float endTime = Time.unscaledTime + duration;
        Vector2 distance = rt.anchoredPosition - targetPosition;
        while (endTime > Time.unscaledTime) {
            rt.anchoredPosition = targetPosition + distance * Mathf.Pow((endTime - Time.unscaledTime) / duration, 3f);
            yield return null;
        }
        rt.anchoredPosition = targetPosition;
    }

    /// <summary>
    /// Slows time incrementally until pause
    /// </summary>
    /// <param name="transitionDuration"> Smooth duration it takes to reach minTimeScale </param>
    /// <param name="minTimeScale"> target time scale </param>
    private IEnumerator SlowPause(float transitionDuration, float minTimeScale = 0) {
        float endTime = Time.unscaledTime + transitionDuration;
        float distance = Time.timeScale - minTimeScale;
        while (endTime > Time.unscaledTime) {
            Time.timeScale = minTimeScale + distance * Mathf.Pow((endTime - Time.unscaledTime) / transitionDuration, 3f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = minTimeScale;
    }

    private void InvokeUnscaled(UnityAction call, float duration) {
        StartCoroutine(InvokeUnscaledCoroutine(call, duration));
    }

    private IEnumerator InvokeUnscaledCoroutine(UnityAction call, float duration) {
        float endTime = Time.unscaledTime + duration;
        while (endTime > Time.unscaledTime)
            yield return null;
        call.Invoke();
    }

    // helper method that checks and sets up winning conditions for progress screen UI.
    private void RunEndgameChecksAndSetup() {
        // bool gameWon = false;
        int numOfMaxRankPlayers = 0;
        foreach (Player player in players) {
            if (!PlayerUIs[player].winTextController.alreadySetUp)
                PlayerUIs[player].winTextController.SetupWinText(player);
            if (player.rank >= GameManager.instance.maxRank) {
                GameWon = true;
                numOfMaxRankPlayers++;
                PlayerUIs[player].GO_progressBar.GetComponent<ProgressBarAlphaController>().FadeInProgressBar();
                //PlayerUIs[player].ExperianceSlider.GetComponentInChildren<Outline>().enabled = true;
                PlayerUIs[player].winTextController.TurnOnWinText();
                // PlayerUIs[player].winTextController.SetupWinText(players[i]);
            }
            else {
                PlayerUIs[player].winTextController.TurnOffWinText();
            }
        }

        if (GameWon) {
            Debug.Log("game won");
            foreach (Player player in players) {
                if (player.rank < GameManager.instance.maxRank) {
                    Debug.Log("Player " + player.playerNumber + " should be faded out");
                    PlayerUIs[player].GO_progressBar.GetComponent<ProgressBarAlphaController>().FadeOutProgressBar();
                }
            }
        }
    }

    private class PlayerProgressObject {
        public readonly GameObject GO_progressBar;
        public readonly ProgressBarWinTextController winTextController;
        //public float Experiance { get { return ExperianceSlider.value; } }
        public ProgressScreen_EXPBar expBar;

        public PlayerProgressObject(GameObject progressBar, int maxPoints) {
            GO_progressBar = progressBar;
            expBar = progressBar.GetComponent<ProgressScreen_EXPBar>();
            expBar.SetUp(maxPoints);
            winTextController = progressBar.GetComponent<ProgressBarWinTextController>();
            //ExperianceSlider.gameObject.GetComponentInChildren<Outline>().enabled = false;
        }
    }
}
