using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UI_FitInParent : MonoBehaviour
{
    public float minAspectRatio;
    public float maxAspectRatio;
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
    }

    void Update() {
        switch(GetState()) {
            case ScaleState.min:
                rectTransform.sizeDelta = new Vector2(parentRectTransform.rect.width, parentRectTransform.rect.width / minAspectRatio);
                break;
            case ScaleState.max:
                rectTransform.sizeDelta = new Vector2(parentRectTransform.rect.height * maxAspectRatio, parentRectTransform.rect.height);
                break;
            case ScaleState.middle:
                rectTransform.sizeDelta = parentRectTransform.rect.size;
                break;
        }
    }

    private ScaleState GetState() {
        float parentAspectRatio = parentRectTransform.rect.width / parentRectTransform.rect.height;
        if (parentAspectRatio < minAspectRatio)
            return ScaleState.min;
        else if (parentAspectRatio > maxAspectRatio)
            return ScaleState.max;
        return ScaleState.middle;
    }

    private enum ScaleState { min, middle, max }
}
