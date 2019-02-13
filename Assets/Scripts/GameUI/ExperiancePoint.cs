using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ExperiancePoint : MonoBehaviour
{
    private ProgressScreen_EXPBar expBar;
    private RectTransform rectTransform;
    private Image image;

    public int points = 1;

    // Start is called before the first frame update
    void Awake()
    {
        expBar = GetComponentInParent<ProgressScreen_EXPBar>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate() {
        rectTransform.sizeDelta = new Vector2(expBar.PointWidth * points, rectTransform.sizeDelta.y);
    }

    public void SetExperiance(GameManager.GameRound.BonusExperiance experiance) {
        points = experiance.Points;
        image.color = experiance.Color;
    }
}
