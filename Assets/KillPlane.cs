using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {

    public GameObject OffScreenDeathEffect;

    private void OnTriggerEnter(Collider other) {
        Player player;
        if ((player = other.GetComponent<Player>()) != null) {
            player.Kill();
            CameraShake.ShakeDiminish(4, 1);
            Instantiate<GameObject>(OffScreenDeathEffect, other.transform.position, Quaternion.identity);
        }
        else
            Destroy(other.gameObject);
    }
}
