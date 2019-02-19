using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressScreen_EXPBar : MonoBehaviour {

    [SerializeField]
    private RectTransform BaseRect;
    [SerializeField]
    private RectTransform FillRect;
    [SerializeField]
    private RectTransform HandleRect;

    public int MaxPoints { get; private set; }
    public float PointHeight {
        get {
            return FillRect.rect.height / MaxPoints;
        }
    }

    public ExperiancePoint experiancePointPrefab;

    public void SetColor(Color color) {
        HandleRect.GetComponent<Image>().color = color;
    }

    private void LateUpdate() {
        if (HandleRect != null) {
            float yPosition = 0;
            if (FillRect.childCount > 0) {
                RectTransform lastChild = FillRect.GetChild(FillRect.childCount - 1).GetComponent<RectTransform>();
                yPosition = (lastChild.rect.height / 2 - lastChild.anchoredPosition.y) / BaseRect.rect.height;
            }
            HandleRect.anchorMax = new Vector2(1, yPosition);
            HandleRect.anchorMin = new Vector2(0, yPosition);
            HandleRect.anchoredPosition = Vector2.zero;
        }
    }

    public void SetUp(int maxPoints) {
        MaxPoints = maxPoints;
    }

    public void AddPoints(GameManager.GameRound.BonusExperiance experiance) {
        Instantiate<GameObject>(experiancePointPrefab.gameObject, FillRect).GetComponent<ExperiancePoint>().SetExperiance(experiance);
    }
}
