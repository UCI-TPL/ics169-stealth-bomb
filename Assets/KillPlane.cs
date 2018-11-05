using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        Player player;
        if ((player = other.GetComponent<Player>()) != null)
            player.Kill();
        else
            Destroy(other.gameObject);
    }
}
