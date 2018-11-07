using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetsCamera : MonoBehaviour {

    public List<GameObject> targets;
    private Vector3 currentVelocity;
    [Tooltip("Time it takes for the camera to move to it's target destination")]
    public float smoothTime = 1f;

	void LateUpdate () {
        if (targets.Count > 0) {
            Vector3 averagePos = Vector3.zero;
            for (int i = targets.Count - 1; i >= 0; --i) {
                if (targets[i] == null)
                    targets.RemoveAt(i);
                else
                    averagePos += targets[i].transform.position;
            }
            if (targets.Count > 0) {
                averagePos /= targets.Count;
                transform.position = Vector3.SmoothDamp(transform.position, averagePos, ref currentVelocity, smoothTime);
            }
        }
	}
}
