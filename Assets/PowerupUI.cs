using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupUI : MonoBehaviour {

    public Player player;
    public RectTransform powerupDisplay;
    public int numToDisplay;
    private Queue<RectTransform> powerUpDisplays;
    private RectTransform rectTransform;

    private Rect size;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        size = rectTransform.rect;
        powerUpDisplays = new Queue<RectTransform>();
        for (int i = 0; i < numToDisplay; ++i) {
            RectTransform rt = Instantiate(powerupDisplay.gameObject, transform).GetComponent<RectTransform>();
            powerUpDisplays.Enqueue(rt);
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (size != rectTransform.rect) {
            UpdateUI();
            size = rectTransform.rect;
        }
    }

    private void UpdateUI() {
        float height = rectTransform.rect.height;
        for (int i = 0; i < numToDisplay; ++i) {
            RectTransform rt = powerUpDisplays.ToArray()[i];
            rt.position = rectTransform.position;
            rt.sizeDelta = new Vector2(height, height);
        }
    }
}
