using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour {

    [SerializeField]
    private ItemData data;

    private bool destroying = false;

    private void Awake() {
        if (data != null) // Set item sprite if item data already present, this would only happen is item was placed in level by editor
            GetComponent<SpriteRenderer>().sprite = data.image;
    }

    private void Start() {
        if (data == null) // Check to ensure item is present
            Debug.LogError(gameObject.name + " missing Item");
        StartCoroutine("DefaultCreateEff");
    }

    public void SetItemData(ItemData data) {
        this.data = data;
        GetComponent<SpriteRenderer>().sprite = data.image;
    }

    // If collider is a player, add item to player
    private void OnTriggerEnter(Collider other) {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null) {
            playerController.player.AddItem(data);
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
        transform.GetChild(0).GetComponent<Collider>().enabled = false; // Disable further collision detections while destroying
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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
