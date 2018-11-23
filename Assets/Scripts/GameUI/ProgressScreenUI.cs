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

    private Dictionary<Player, PlayerProgressObject> PlayerUIs = new Dictionary<Player, PlayerProgressObject>();

    // Use this for initialization
    void Awake () {
        canvas = GetComponent<Canvas>();
    }

    private void Update() {
        // Fix ruler layout
        RulerTextLayoutElement.minWidth = RulerTextLayoutElement.preferredWidth = PlayerIconsRect.rect.width;
    }

    /// <summary>
    /// Displays the progress screen overlay, and pauses game.
    /// </summary>
    /// <param name="round"> Gameround the progress screen is to display </param>
    /// <param name="action"> Event thats invoked when the progress screen is closed. </param>
    public void StartProgressScreen(GameManager.GameRound round, UnityAction action = null) {
        DisplayScreen(1f);
        StartCoroutine(SlowPause(2, 0.05f));
        SetUpProgressScreen(round);

        InvokeUnscaled(UpdateExperiance, 1f);

        // Setup the closing the progress screen by pressing start
        StartCoroutine(InvokeOnPressStart(round.players, delegate {
            StopAllCoroutines(); // Ensure all coroutines and invokes are reset
            CancelInvoke();
            Time.timeScale = 1;
            action.Invoke();
            ProgressScreenRect.gameObject.SetActive(false);
        }));
    }

    private void UpdateExperiance() {
        foreach (KeyValuePair<Player, PlayerProgressObject> pair in PlayerUIs) {
            StartCoroutine(ExperianceOverTime(pair.Value.ExperianceSlider, pair.Key.experiance));
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
    /// Invokes the event after any of the listed players press start
    /// </summary>
    /// <param name="players"> List of players to listen to </param>
    /// <param name="unityEvent"> Event to be invoked </param>
    private IEnumerator InvokeOnPressStart(Player[] players, UnityAction action) {
        bool pressedStart = false;
        UnityAction checkStart = delegate { pressedStart = true; };
        foreach (Player player in players)
            InputManager.inputManager.controllers[player.playerNumber].start.OnDown.AddListener(checkStart);
        while (!pressedStart)
            yield return null;
        foreach (Player player in players)
            InputManager.inputManager.controllers[player.playerNumber].start.OnDown.RemoveListener(checkStart);
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
        foreach (KeyValuePair<Player, PlayerProgressObject> pair in PlayerUIs) { // Destroy previous player icons
            Destroy(pair.Value.GO_icon);
            Destroy(pair.Value.GO_progressBar);
        }
        PlayerUIs.Clear();
        foreach (Player player in round.players) { // Create new player icons with player colors
            GameObject playerIcon = Instantiate<GameObject>(PlayerIconPrefab, PlayerIconsRect);
            playerIcon.GetComponent<Image>().color = player.Color;
            GameObject playerSlider = Instantiate<GameObject>(ProgressScreenSliderPrefab, ProgressScreenSliderRect);
            PlayerProgressObject playerProgressObject = new PlayerProgressObject(playerIcon, playerSlider);
            playerProgressObject.ExperianceSlider.maxValue = GameManager.instance.maxRank;
            playerProgressObject.ExperianceSlider.value = round.initialExperiance[player];
            PlayerUIs.Add(player, playerProgressObject);
        }
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

    private class PlayerProgressObject {
        public readonly GameObject GO_icon;
        public readonly GameObject GO_progressBar;
        public readonly Slider ExperianceSlider;

        public PlayerProgressObject(GameObject icon, GameObject progressBar) {
            GO_icon = icon;
            GO_progressBar = progressBar;
            ExperianceSlider = progressBar.GetComponentInChildren<Slider>();
        }
    }
}
