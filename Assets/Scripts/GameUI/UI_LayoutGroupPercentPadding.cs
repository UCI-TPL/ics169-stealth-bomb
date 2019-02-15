using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(LayoutGroup))]
public class UI_LayoutGroupPercentPadding : MonoBehaviour {

    private LayoutGroup layoutGroup;
    [Header("Padding")]
    public float left;
    public float right;
    public float top;
    public float bottom;
    public float spacing;
    public Vector2 gridSpacing;
    private RectTransform rectTransform;
    private LayoutGroupType layoutGroupType;
    private HorizontalLayoutGroup horizontalLayoutGroup;
    private VerticalLayoutGroup verticalLayoutGroup;
    private GridLayoutGroup gridLayoutGroup;

    void Start() {
        layoutGroup = GetComponent<LayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        // Save type of layout group so it does not have to be checked again later
        if (layoutGroup.GetType() == typeof(HorizontalLayoutGroup)) {
            layoutGroupType = LayoutGroupType.Horizontal;
            horizontalLayoutGroup = (HorizontalLayoutGroup)layoutGroup;
        }
        else if (layoutGroup.GetType() == typeof(VerticalLayoutGroup)) {
            layoutGroupType = LayoutGroupType.Vertical;
            verticalLayoutGroup = (VerticalLayoutGroup)layoutGroup;
        }
        else if (layoutGroup.GetType() == typeof(GridLayoutGroup)) {
            layoutGroupType = LayoutGroupType.Grid;
            gridLayoutGroup = (GridLayoutGroup)layoutGroup;
        }
    }
    
    void Update() {
        layoutGroup.padding = new RectOffset(Mathf.RoundToInt(left * rectTransform.rect.width), Mathf.RoundToInt(right * rectTransform.rect.width), Mathf.RoundToInt(top * rectTransform.rect.height), Mathf.RoundToInt(bottom * rectTransform.rect.height));
        // Modify spacing depending on layout group type
        switch (layoutGroupType) {
            case LayoutGroupType.Horizontal:
                horizontalLayoutGroup.spacing = spacing * rectTransform.rect.width;
                break;
            case LayoutGroupType.Vertical:
                verticalLayoutGroup.spacing = spacing * rectTransform.rect.height;
                break;
            case LayoutGroupType.Grid:
                gridLayoutGroup.spacing = gridSpacing * rectTransform.rect.size;
                break;
        }
    }

    private enum LayoutGroupType { Horizontal, Vertical, Grid }
}
