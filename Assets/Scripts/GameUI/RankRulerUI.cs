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
        int totalRange = maxRank - minRank;
        float separator = 1f / totalRange;
        for (int i = totalRange; i >= 0; --i) {
            RectTransform newTick;
            if (createdTicks.Count <= totalRange - i) {
                newTick = Instantiate<GameObject>(rulerTickPrefab, transform).GetComponent<RectTransform>();
                createdTicks.Add(newTick);
            }
            else {
                newTick = createdTicks[totalRange - i];
                newTick.gameObject.SetActive(true);
            }
            newTick.anchorMin = new Vector2(separator * i, 0);
            newTick.anchorMax = new Vector2(separator * i, 1);
            newTick.anchoredPosition = Vector2.zero;
            newTick.GetComponentInChildren<Text>().text = (minRank + i).ToString();
        }
        for (int i = totalRange; i < createdTicks.Count; ++i)
            createdTicks[i].gameObject.SetActive(false);
    }
}
