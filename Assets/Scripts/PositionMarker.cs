using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionMarker : MonoBehaviour {
    private RectTransform canvas;
    private Image circleImage;
    private RectTransform imageRect;

    [SerializeField]
    private LayerMask groundLayer;

    [Header("Image Settings")]
    [SerializeField]
    private Sprite sprite;
    public Color color;
    [SerializeField]
    private float minDistance = 0, maxDistance = 4.5f;
    [SerializeField]
    private Gradient colorGradient;
    [SerializeField]
    private AnimationCurve sizeCurve;

    [Header("Canvas Settings")]
    [SerializeField]
    private int orderInLayer = -10;
    [SerializeField]
    private float heightOffset = 0.1f;

    //private Vector2 lastPos1 = Vector2.zero, lastPos2 = Vector2.zero, lastPos3 = Vector2.zero, lastPos4 = Vector2.zero, lastPos5 = Vector2.zero, lastPos6 = Vector2.zero, currPos = Vector2.zero;

    // Set up child components
    void Awake() {
        GameObject c = new GameObject("Canvas");
        c.AddComponent<Canvas>().sortingOrder = orderInLayer;
        canvas = c.GetComponent<RectTransform>();
        canvas.SetParent(transform);
        canvas.localPosition = Vector3.zero;
        canvas.localRotation = Quaternion.Euler(90, 0, 0);
        canvas.sizeDelta = new Vector2(2, 2);

        GameObject imageObj = new GameObject("CircleImage");
        circleImage = imageObj.AddComponent<Image>();
        circleImage.sprite = sprite;
        imageRect = imageObj.GetComponent<RectTransform>();
        imageRect.SetParent(canvas);
        imageRect.localPosition = Vector3.zero;
        imageRect.localRotation = Quaternion.identity;
        imageRect.sizeDelta = Vector2.one;
    }

    // Place marker on the ground
    void LateUpdate() {
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxDistance, groundLayer, QueryTriggerInteraction.Ignore);
        circleImage.gameObject.SetActive(hit.distance > minDistance);
        if (hit.distance > minDistance) {
            canvas.position = hit.point + transform.up * heightOffset;
            float distanceValue = (hit.distance - minDistance) / (maxDistance - minDistance);
            // Set image color based on gradient evaluated by distance
            circleImage.color = color * colorGradient.Evaluate(distanceValue);
            // Set image size based on gradient evaluated by distance
            imageRect.sizeDelta = Vector2.one * sizeCurve.Evaluate(distanceValue);

            // Testing how it looks to have the marker stretch with movement
            //imageRect.rotation = Quaternion.Euler(90, 0, 0) * Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, currPos - lastPos1), Vector3.forward);
            //imageRect.sizeDelta = new Vector2(imageRect.sizeDelta.x, imageRect.sizeDelta.y * (1 + 0.2f * Vector2.Distance(lastPos1, currPos)));
        }
    }

    //private void FixedUpdate() {
    //    lastPos1 = lastPos2;
    //    lastPos2 = lastPos3;
    //    lastPos3 = lastPos4;
    //    lastPos4 = lastPos5;
    //    lastPos5 = lastPos6;
    //    lastPos6 = currPos;
    //    currPos = new Vector2(transform.position.x, transform.position.z);
    //}
}
