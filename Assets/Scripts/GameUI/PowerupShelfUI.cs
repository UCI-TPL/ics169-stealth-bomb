using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupShelfUI : MonoBehaviour {

    public Player player;
    public RectTransform powerupDisplay;
    public int numToDisplay;
    public static float AnimationDuration = 0.25f;
    private RectTransform rectTransform;
    private Transform activePool;
    private Transform inactivePool;

    private Rect size;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        size = rectTransform.rect;
        activePool = new GameObject("Active Pool").transform;
        activePool.parent = transform;
        inactivePool = new GameObject("Inactive Pool").transform;
        inactivePool.parent = transform;
        inactivePool.gameObject.SetActive(false);

        for (int i = 0; i < numToDisplay; ++i)
            Instantiate(powerupDisplay.gameObject, inactivePool).GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (size != rectTransform.rect) {
            size = rectTransform.rect;
            ScaleUI();
        }
    }

    private void ScaleUI() {
        float length = size.width - size.height;
        float unit = length / (numToDisplay - 3.5f);
        float startPos = rectTransform.position.x - length / 2;
        for (int i = 0; i < activePool.childCount; ++i) {
            RectTransform rt = activePool.GetChild(i).GetComponent<RectTransform>();
            StartCoroutine(MovePowerup(rt, new Vector3(startPos + TargetPosition(i) * unit, rectTransform.position.y, rectTransform.position.z), AnimationDuration));
            rt.sizeDelta = new Vector2(size.height, size.height);
        }
    }
    private void UpdateDisplaysPositions() {
        float length = size.width - size.height;
        float unit = length / (numToDisplay - 3.5f);
        float startPos = rectTransform.position.x - length / 2;
        for (int i = 0; i < activePool.childCount; ++i) {
            RectTransform rt = activePool.GetChild(i).GetComponent<RectTransform>();
            StartCoroutine(MovePowerup(rt, new Vector3(startPos + TargetPosition(i) * unit, rectTransform.position.y, rectTransform.position.z), AnimationDuration));
        }
    }

    // Move to a position over a duration and slow down at the end
    private IEnumerator MovePowerup(RectTransform rt, Vector3 targetPosition, float duration) {
        float endTime = Time.time + duration;
        Vector3 distance = rt.position - targetPosition;
        while (endTime > Time.time) {
        rt.position = targetPosition + distance * Mathf.Pow((endTime - Time.time)/duration, 2f);
            yield return null;
        }
        rt.position = targetPosition;
    }

    private float TargetPosition(float input) {
        input += Mathf.Min(numToDisplay - activePool.childCount, 3);
        if (input > 3) // From 3 on, powerups should move linearly.  These are the powerups that are not obstructed
            return input - 2.5f;
        else if (input < 1) // From 1 to 0 powerup should not move, stay at 0
            return 0;
        // 1 to 3 powerups start to slow down as they approach the end, this results in powerups stacking together at the end
        return -Mathf.Log(-input + 3.19f) / 5 + 0.16f; // y = -ln(-x+3.19)/5+0.16
        //return Mathf.Pow(2.5f, 2 * input - 4.8f); // y = 2.5^(2x-4.8)
    }

    public void AddPowerup(PowerupData powerupData, Buff buff) {
        StopAllCoroutines();
        RectTransform newPowerup;
        if (inactivePool.childCount > 0) {
            newPowerup = inactivePool.GetChild(0).GetComponent<RectTransform>();
            newPowerup.SetParent(activePool, false);
        } else
            newPowerup = activePool.GetChild(0).GetComponent<RectTransform>();
        newPowerup.SetAsLastSibling();
        newPowerup.position = new Vector3((rectTransform.position.x - (size.width - size.height) / 2) + TargetPosition(activePool.childCount - 1) * ((size.width - size.height) / (numToDisplay - 3.5f)), rectTransform.position.y, rectTransform.position.z);
        newPowerup.GetComponent<PowerupDisplayUI>().NewPowerup(powerupData, buff, new Vector2(size.height, size.height), AnimationDuration);
        UpdateDisplaysPositions();
    }
}
