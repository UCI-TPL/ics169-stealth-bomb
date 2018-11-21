using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankRulerUI : MonoBehaviour {

    [SerializeField]
    private GameObject rulerTickPrefab;
    private RectTransform rectTransform;
    private List<RectTransform> createdTicks = new List<RectTransform>();

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetRange(int minRank, int maxRank) {
        foreach (RectTransform tick in createdTicks)
            Destroy(tick.gameObject);
        createdTicks.Clear();
        float separator = 1f / (maxRank - minRank);
        for (int i = maxRank - minRank; i >= 0; --i) {
            RectTransform newTick = Instantiate<GameObject>(rulerTickPrefab, transform).GetComponent<RectTransform>();
            newTick.anchorMin = new Vector2(separator * i, 0);
            newTick.anchorMax = new Vector2(separator * i, 1);
            newTick.anchoredPosition = Vector2.zero;
            newTick.GetComponentInChildren<Text>().text = (minRank + i).ToString();
            createdTicks.Add(newTick);
        }
    }
}
