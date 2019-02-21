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
                RectTransform topChild = FillRect.GetChild(FillRect.childCount - 1).GetComponent<RectTransform>();
                float topPosition = topChild.rect.height / 2 - topChild.anchoredPosition.y;
                for (int i = FillRect.childCount - 1; i > 0; --i) {
                    RectTransform underChild = FillRect.GetChild(i - 1).GetComponent<RectTransform>();
                    if (topPosition > (underChild.rect.height / 2 - underChild.anchoredPosition.y))
                        break;
                    topChild = FillRect.GetChild(i-1).GetComponent<RectTransform>();
                    topPosition = topChild.rect.height / 2 - topChild.anchoredPosition.y;
                }
                yPosition = topPosition / BaseRect.rect.height;
            }
            HandleRect.anchorMax = new Vector2(1, yPosition);
            HandleRect.anchorMin = new Vector2(0, yPosition);
            HandleRect.anchoredPosition = Vector2.zero;
        }
    }

    public void SetUp(int maxPoints) {
        MaxPoints = maxPoints;
    }

    public void AddPoints(GameManager.GameRound.BonusExperiance experiance, float animationDuration = 0) {
        Instantiate<GameObject>(experiancePointPrefab.gameObject, FillRect).GetComponent<ExperiancePoint>().SetExperiance(experiance, animationDuration);
    }
}
