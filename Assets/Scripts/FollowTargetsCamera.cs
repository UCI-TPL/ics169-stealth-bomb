using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetsCamera : MonoBehaviour {

    public Camera targetCamera;
    public List<GameObject> targets;
    private Vector3 currentVelocity;
    [Tooltip("Time it takes for the camera to move to it's target destination")]
    public float smoothTime = 1f;
    public float minZoom;
    public float maxZoom;
    [Range(0, 1)]
    public float minZoomDistance;
    [Range(0, 1)]
    public float maxZoomDistance;

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
                float zoomLevel = Mathf.Clamp01(Mathf.InverseLerp(minZoom, maxZoom, Mathf.Max(size.x, size.y)));
                targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, Mathf.Lerp(minZoom, maxZoom, zoomLevel), Time.deltaTime);
            }
        }
	}

    private static Vector3 ScaleToCamera(Vector3 worldPosition, Camera camera) {
        return Vector3.Scale(camera.WorldToScreenPoint(worldPosition), new Vector3(1 / (float)camera.pixelWidth, 1 / (float)camera.pixelHeight, 0));
    }
}
