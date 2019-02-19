using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UI_VerticalSpacing : MonoBehaviour {

    private RectTransform rectTransform;
    [SerializeField]
    private float maxAspectRatio;

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate() {
        float maxHeight = rectTransform.rect.height / rectTransform.childCount;
        for (int i = 0; i < rectTransform.childCount; ++i) {
            RectTransform crt = rectTransform.GetChild(i).GetComponent<RectTransform>();
            float yAnchor = 1 - (float)i / rectTransform.childCount;
            crt.anchorMax = new Vector2(1, yAnchor);
            crt.anchorMin = new Vector2(0, yAnchor);
            crt.sizeDelta = new Vector2(0, Mathf.Min(maxHeight, rectTransform.rect.width / maxAspectRatio));
            crt.anchoredPosition = Vector2.zero;
        }
    }
}
