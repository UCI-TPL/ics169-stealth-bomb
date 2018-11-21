using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(RectTransform))]
public class LayoutElementAspectRatioFitter : MonoBehaviour {

    public AspectRatioFitter.AspectMode AspectMode;
    public float AspectRatio = 1;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;

    void Start() {
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update () {
        switch (AspectMode) {
            //case AspectRatioFitter.AspectMode.EnvelopeParent:
            //    if (parentRect.rect.height * AspectRatio > parentRect.rect.width)
            //        HeightControlsWidth(rectTransform.rect.height);
            //    else
            //        WidthControlsHeight(rectTransform.rect.width);
            //    break;
            //case AspectRatioFitter.AspectMode.FitInParent:
            //    if (parentRect.rect.height * AspectRatio > parentRect.rect.width)
            //        WidthControlsHeight(rectTransform.rect.width);
            //    else
            //        HeightControlsWidth(rectTransform.rect.height);
            //    break;
            case AspectRatioFitter.AspectMode.HeightControlsWidth:
                HeightControlsWidth(rectTransform.rect.height);
                break;
            case AspectRatioFitter.AspectMode.None:
                break;
            case AspectRatioFitter.AspectMode.WidthControlsHeight:
                WidthControlsHeight(rectTransform.rect.width);
                break;
        }
    }

    private void WidthControlsHeight(float width) {
        layoutElement.preferredWidth = -1;
        layoutElement.flexibleWidth = 1;
        layoutElement.flexibleHeight = -1;
        layoutElement.preferredHeight = width / AspectRatio;
    }

    private void HeightControlsWidth(float height) {
        layoutElement.preferredHeight = -1;
        layoutElement.flexibleWidth = -1;
        layoutElement.flexibleHeight = 1;
        layoutElement.preferredWidth = height * AspectRatio;
    }
}
