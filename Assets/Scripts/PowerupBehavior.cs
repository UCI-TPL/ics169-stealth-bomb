using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehavior : MonoBehaviour {

    [SerializeField]
    private Powerup powerup;

    private bool destroying = false;

    private void Awake() {
        if (powerup == null) // Check to ensure Powerup is present
            Debug.LogError(gameObject.name + " missing Powerup");
    }

    // If collider is a player, add powerup to player
    private void OnTriggerEnter(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            player.AddPowerup(powerup);
            Destroy();
        }
    }

    // Destroy the object;
    public void Destroy() {
        if (!destroying) { // Ensure Destroy() is only called once
            destroying = true;
            StartCoroutine("DefaultDestroyEff"); // Play destroy effect
        }
    }

    // Effect that plays while the object is destroying
    private IEnumerator DefaultDestroyEff() {
        GetComponent<Collider>().enabled = false; // Disable further collision detections while destroying
        float curTime = Time.time;
        while (true) {
            float scale = -Mathf.Pow((4f * (Time.time - curTime - 0.125f)), 2) + 1.25f;
            transform.localScale = new Vector3(scale, scale, scale);
            if (scale <= 0)
                break;
            yield return null;
        }
        Destroy(gameObject);
    }
}
