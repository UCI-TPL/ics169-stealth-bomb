using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
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
    [SerializeField]
    private RectTransform PlayerIconsRect;
    [SerializeField]
    private GameObject PlayerIconPrefab;
    [SerializeField]
    private RectTransform ProgressScreenSliderRect;
    [SerializeField]
    private GameObject ProgressScreenSliderPrefab;

    [Header("Ruler Settings")]
    [SerializeField]
    private RankRulerUI rankRulerUI;
    [SerializeField]
    private LayoutElement RulerTextLayoutElement;

    private List<PlayerProgressObject> PlayerUIs = new List<PlayerProgressObject>();
    private Player[] players;

    [HideInInspector]
    public bool GameWon { get; private set; }

    // Use this for initialization
    void Awake () {
        canvas = GetComponent<Canvas>();
        GameWon = false;
    }

    private void Update() {
        // Fix ruler layout
        RulerTextLayoutElement.minWidth = RulerTextLayoutElement.preferredWidth = PlayerIconsRect.rect.width;

        UpdateCrown();
    }

    /// <summary>
    /// Displays the progress screen overlay, and pauses game.
    /// </summary>
    /// <param name="round"> Gameround the progress screen is to display </param>
    /// <param name="action"> Event thats invoked when the progress screen is closed. </param>
    public void StartProgressScreen(GameManager.GameRound round, UnityAction action = null) {
        players = round.players; // Save list of players
        DisplayScreen(1f);
        StartCoroutine(SlowPause(2, 0.05f));
        SetUpProgressScreen(round);

        InvokeUnscaled(UpdateExperiance, 1f);

        // Setup the closing the progress screen by pressing start
        StartCoroutine(InvokeOnPressStart(round.players, delegate {
            StopAllCoroutines(); // Ensure all coroutines and invokes are reset
            CancelInvoke();
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
            action.Invoke();
            ProgressScreenRect.gameObject.SetActive(false);
        }));
    }

    private void UpdateExperiance() {
        for (int i = 0; i < players.Length; ++i) {
            StartCoroutine(ExperianceOverTime(PlayerUIs[i].ExperianceSlider, players[i].experiance));
        }
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
    public void UpdateCrown() {
        float highestEXP = 0;
        PlayerProgressObject leader = null;
        foreach (PlayerProgressObject player in PlayerUIs) {
            player.expBar.IsLeader = false;
            if (player.Experiance > highestEXP) {
                highestEXP = player.Experiance;
                leader = player;
            }
            else if (player.Experiance == highestEXP)
                leader = null;
        }
        if (leader != null)
            leader.expBar.IsLeader = true;
    }

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
        Vector3 targetPosition = ProgressScreenRect.position; // saves the default position as the target position
        ProgressScreenRect.anchoredPosition = new Vector2(0, -canvas.GetComponent<RectTransform>().rect.height); // Places the screen just under the screen out of view
        StartCoroutine(MoveSmooth(ProgressScreenRect, targetPosition, animDuration));
    }

    private void SetUpProgressScreen(GameManager.GameRound round) {
        for (int i = 0; i < players.Length; ++i) {
            PlayerProgressObject playerProgressObject;
            if (PlayerUIs.Count <= i) { // If there are not enough already created Player UIs then make a new one
                GameObject playerIcon = Instantiate<GameObject>(PlayerIconPrefab, PlayerIconsRect);
                GameObject playerSlider = Instantiate<GameObject>(ProgressScreenSliderPrefab, ProgressScreenSliderRect);
                playerProgressObject = new PlayerProgressObject(playerIcon, playerSlider);
                PlayerUIs.Add(playerProgressObject);
            }
            else { // Reuse previously created PlayerUIs
                playerProgressObject = PlayerUIs[i];
                playerProgressObject.GO_icon.SetActive(true);
                playerProgressObject.GO_progressBar.SetActive(true);
            }
            playerProgressObject.GO_icon.GetComponent<Image>().color = players[i].Color;
            playerProgressObject.ExperianceSlider.maxValue = GameManager.instance.maxRank;
            playerProgressObject.ExperianceSlider.value = round.initialExperiance[players[i]];
        }

        // players should only be able to reach rank 10 once per game before being forced to head back to main menu.
        RunEndgameChecksAndSetup();
        // if (numOfMaxRankPlayers > )
        rankRulerUI.SetRange(0, GameManager.instance.maxRank); // Setup ruler to display number of ranks
    }

    // Move to a position over a duration and slowing down near end
    private IEnumerator MoveSmooth(RectTransform rt, Vector3 targetPosition, float duration) {
        float endTime = Time.unscaledTime + duration;
        Vector3 distance = rt.position - targetPosition;
        while (endTime > Time.unscaledTime) {
            rt.position = targetPosition + distance * Mathf.Pow((endTime - Time.unscaledTime) / duration, 3f);
            yield return null;
        }
        rt.position = targetPosition;
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
        for (int i = 0; i < players.Length; i++) {
            if (!PlayerUIs[i].winTextController.alreadySetUp)
                PlayerUIs[i].winTextController.SetupWinText(players[i]);
            if (players[i].rank >= GameManager.instance.maxRank) {
                GameWon = true;
                numOfMaxRankPlayers++;
                PlayerUIs[i].GO_progressBar.GetComponent<ProgressBarAlphaController>().FadeInProgressBar();
                PlayerUIs[i].ExperianceSlider.GetComponentInChildren<Outline>().enabled = true;
                PlayerUIs[i].winTextController.TurnOnWinText();
                // PlayerUIs[i].winTextController.SetupWinText(players[i]);
            }
            else {
                PlayerUIs[i].winTextController.TurnOffWinText();
            }
        }

        if (GameWon) {
            Debug.Log("game won");
            for (int i = 0; i < players.Length; i++) {
                if (players[i].rank < GameManager.instance.maxRank) {
                    Debug.Log("Player " + i + " should be faded out");
                    PlayerUIs[i].GO_progressBar.GetComponent<ProgressBarAlphaController>().FadeOutProgressBar();
                }
            }
        }
    }

    private class PlayerProgressObject {
        public readonly GameObject GO_icon;
        public readonly GameObject GO_progressBar;
        public readonly Slider ExperianceSlider;
        public readonly ProgressBarWinTextController winTextController;
        public float Experiance { get { return ExperianceSlider.value; } }
        public ProgressScreen_EXPBar expBar;

        public PlayerProgressObject(GameObject icon, GameObject progressBar) {
            GO_icon = icon;
            GO_progressBar = progressBar;
            ExperianceSlider = progressBar.GetComponentInChildren<Slider>();
            expBar = progressBar.GetComponent<ProgressScreen_EXPBar>();
            winTextController = progressBar.GetComponent<ProgressBarWinTextController>();
            ExperianceSlider.gameObject.GetComponentInChildren<Outline>().enabled = false;
        }
    }
}
