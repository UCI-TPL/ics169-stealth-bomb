using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {

    public GameObject OffScreenDeathEffect;

    private void OnTriggerEnter(Collider other) {
        PlayerController playerController;
        if ((playerController = other.GetComponent<PlayerController>()) != null) {
            playerController.Kill();
            CameraShake.ShakeDiminish(4, 1);
            Instantiate<GameObject>(OffScreenDeathEffect, other.transform.position, Quaternion.identity);
        }
    }
}
