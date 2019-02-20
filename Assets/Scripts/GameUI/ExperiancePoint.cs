using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ExperiancePoint : MonoBehaviour
{
    private ProgressScreen_EXPBar expBar;
    private RectTransform rectTransform;
    private Image image;

    private float currentPoints;
    public int Points { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        expBar = GetComponentInParent<ProgressScreen_EXPBar>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, expBar.PointHeight * currentPoints);
    }

    public void SetExperiance(GameManager.GameRound.BonusExperiance experiance, float animationDuration = 0) {
        Points = experiance.Points;
        image.color = experiance.Color;
        StopAllCoroutines();
        StartCoroutine(AddPointsOverTime(animationDuration));
    }

    private IEnumerator AddPointsOverTime(float duration) {
        float startingPoints = rectTransform.sizeDelta.y / expBar.PointHeight;
        float endTime = Time.unscaledTime + duration;
        while (endTime > Time.unscaledTime) {
            currentPoints = MathHelper.LerpDamped(startingPoints, Points, (duration - endTime + Time.unscaledTime) / duration, 3);
            yield return null;
        }
        currentPoints = Points;
    }
}
