using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScreen_EXPBar : MonoBehaviour {

    [SerializeField]
    private Image Crown;
    [SerializeField]
    private RectTransform BarArea;
    public bool IsLeader { get { return Crown.enabled; } set { Crown.enabled = value; } }

    public int MaxPoints { get; private set; }
    public float PointWidth {
        get {
            return BarArea.rect.width / MaxPoints;
        }
    }

    public ExperiancePoint experiancePointPrefab;

    private void Start() {
        //BarArea = GetComponent<RectTransform>();
    }

    public void SetUp(int maxPoints) {
        MaxPoints = maxPoints;
    }

    public void AddPoints(GameManager.GameRound.BonusExperiance experiance) {
        Instantiate<GameObject>(experiancePointPrefab.gameObject, BarArea).GetComponent<ExperiancePoint>().SetExperiance(experiance);
    }
}
