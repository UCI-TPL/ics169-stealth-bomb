using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehavior : MonoBehaviour {

    [SerializeField]
    private PowerupData powerupData;

    private bool destroying = false;

    private void Awake() {
        if (powerupData != null) // Set power-up sprite if powerup data already present, this would only happen is powerup was place in level by editor
            GetComponent<SpriteRenderer>().sprite = powerupData.image;
    }

    private void Start() {
        if (powerupData == null) // Check to ensure Powerup is present
            Debug.LogError(gameObject.name + " missing Powerup");
        StartCoroutine("DefaultCreateEff");
    }

    public void SetPowerupData(PowerupData data) {
        powerupData = data;
        GetComponent<SpriteRenderer>().sprite = data.image;
    }

    // If collider is a player, add powerup to player
    private void OnTriggerEnter(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            player.AddPowerup(powerupData);
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

    // Effect that plays while the object is creating
    private IEnumerator DefaultCreateEff() {
        GetComponent<Collider>().enabled = false; // Disable collision detections while creating
        float endTime = Time.time + 0.4045085f;
        while (true) {
            float scale = -Mathf.Pow((4f * (endTime - Time.time - 0.125f)), 2) + 1.25f;
            transform.localScale = new Vector3(scale, scale, scale);
            if (endTime <= Time.time)
                break;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
        GetComponent<Collider>().enabled = true; // Enable collisions when done creating
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
