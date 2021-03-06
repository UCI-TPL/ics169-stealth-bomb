﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetsCamera : MonoBehaviour {

    public Camera targetCamera;
    public List<GameObject> targets;
    private Transform Center;
    private Vector3 currentVelocity;
    [Tooltip("Time it takes for the camera to move to it's target destination")]
    public float smoothTime = 1f;
    public float minZoom;
    public float maxZoom;
    public float minZoomDistance;
    public float maxZoomDistance;
    public float anglePerUnit;

    private void Awake() {
        Center = transform.parent;
    }

    void LateUpdate () {
        if (targets.Count > 0) {
            Vector3 averagePos = Vector3.zero;
            bool exist = false;
            Bounds screenSpaceBounds = new Bounds();
            for (int i = targets.Count - 1; i >= 0; --i) {
                if (targets[i] == null)
                    targets.RemoveAt(i);
                else {
                    averagePos += targets[i].transform.position;
                    if (!exist) {
                        screenSpaceBounds = new Bounds(ScaleToCamera(targets[0].transform.position, targetCamera), Vector3.zero);
                        exist = true;
                    }
                    screenSpaceBounds.Encapsulate(ScaleToCamera(targets[i].transform.position, targetCamera));
                }
            }
            if (targets.Count > 0) {
                averagePos /= targets.Count;
                transform.position = Vector3.SmoothDamp(transform.position, averagePos, ref currentVelocity, smoothTime);
                Vector3 size = screenSpaceBounds.size;
                float zoomLevel = Mathf.InverseLerp(minZoomDistance, maxZoomDistance, Mathf.Max(size.x, size.y));
                targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetCamera.transform.parent.position - targetCamera.transform.forward * Mathf.Lerp(minZoom, maxZoom, zoomLevel), Time.deltaTime);
            }

            // Turn Camera based on X offest
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, anglePerUnit * -Vector3.ProjectOnPlane(averagePos, Center.forward).x, 0), Time.deltaTime);
        }
	}

    private static Vector3 ScaleToCamera(Vector3 worldPosition, Camera camera) {
        return Vector3.ProjectOnPlane(worldPosition, camera.transform.forward);
        // return Vector3.Scale(camera.WorldToScreenPoint(worldPosition), new Vector3(1 / (float)camera.pixelWidth, 1 / (float)camera.pixelHeight, 0));
    }
}
