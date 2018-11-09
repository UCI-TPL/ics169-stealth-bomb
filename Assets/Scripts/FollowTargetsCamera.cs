using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetsCamera : MonoBehaviour {

    public Camera camera;
    public List<GameObject> targets;
    private Vector3 currentVelocity;
    [Tooltip("Time it takes for the camera to move to it's target destination")]
    public float smoothTime = 1f;
    public float minZoom;
    public float maxZoom;
    public float maxZoomDistance;

    void LateUpdate () {
        if (targets.Count > 0) {
            Vector3 averagePos = Vector3.zero;
            Bounds screenSpaceBounds = new Bounds(ScaleToCamera(targets[0].transform.position, camera), Vector3.zero);
            for (int i = targets.Count - 1; i >= 0; --i) {
                if (targets[i] == null)
                    targets.RemoveAt(i);
                else {
                    averagePos += targets[i].transform.position;
                    screenSpaceBounds.Encapsulate(ScaleToCamera(targets[i].transform.position, camera));
                }
            }
            if (targets.Count > 0) {
                averagePos /= targets.Count;
                transform.position = Vector3.SmoothDamp(transform.position, averagePos, ref currentVelocity, smoothTime);
                Vector3 size = screenSpaceBounds.size * camera.orthographicSize * 2;
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, Mathf.Lerp(minZoom, maxZoom, Mathf.Max(size.x, size.y) / maxZoomDistance), Time.deltaTime);
            }
        }
	}

    private static Vector3 ScaleToCamera(Vector3 worldPosition, Camera camera) {
        return Vector3.Scale(camera.WorldToScreenPoint(worldPosition), new Vector3(1 / (float)camera.pixelWidth, 1 / (float)camera.pixelHeight, 0));
    }
}
