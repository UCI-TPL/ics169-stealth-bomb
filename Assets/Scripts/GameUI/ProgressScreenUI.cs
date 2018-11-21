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

    [Header("Ruler Settings")]
    [SerializeField]
    private RankRulerUI rankRulerUI;
    [SerializeField]
    private LayoutElement RulerTextLayoutElement;

    // Use this for initialization
    void Awake () {
        canvas = GetComponent<Canvas>();
    }

    private void Update() {
        RulerTextLayoutElement.minWidth = RulerTextLayoutElement.preferredWidth = PlayerIconsRect.rect.width;
    }

    /// <summary>
    /// Displays the progress screen overlay, and pauses game.
    /// </summary>
    /// <returns> Event thats invoked when the progress screen is closed. </returns>
    public UnityEvent StartProgressScreen() {
        UnityEvent closeScreen = new UnityEvent();
        StopAllCoroutines();
        DisplayScreen(1f);
        StartCoroutine(SlowPause(2, 0.05f));
        return closeScreen;
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
        rankRulerUI.SetRange(0, 10);
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
}
