using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector3Extensions;

public class PowerupDebuger : MonoBehaviour {

    [Tooltip("Prefab for power-up select button")]
    public GameObject PowerupSelectButton;
    [Tooltip("Scroll rect that contains power-up select buttons")]
    public RectTransform scrollRect;
    [Tooltip("Image for current selected power-up")]
    public Image selectedImage;
    [Tooltip("List of power-ups")]
    public PowerupList powerupList;
    [Tooltip("Prefab for power-up container")]
    public GameObject powerupContainer;

    private PowerupData selectedPowerup;

    private void Start() {
        SelectPowerup(powerupList.tiers[0].powerups[0]); // Initalize selected powerup to first power-up
        foreach (PowerupList.Tier tier in powerupList.tiers) {
            foreach (PowerupData data in tier.powerups) {
                AddButton(data); // For each power-up in the power-up list create a new selection button
            }
        }
    }

    // Update is called once per frame
    void Update () {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 targetGrid = Vector3.zero;

        RaycastHit hit; // position at mouse cursor
        Tile tile; // Tile at mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit) && (tile = hit.collider.GetComponent<Tile>()) != null) {
            targetGrid = (hit.point - Tile.TileOffset + hit.normal * 0.1f).Round();
            //DrawCubeWithWire(targetGrid + Tile.TileOffset, 1, Color.white, new Color(1, 1, 1, 0.1f));
            if (Input.GetMouseButtonDown(0)) {
                GameObject g = Instantiate(powerupContainer, targetGrid + Tile.TileOffset, Quaternion.identity); // Create a power-up holder
                g.GetComponent<PowerupBehavior>().SetPowerupData(selectedPowerup); // Set the power-up for the power-up holder
            }
        }
    }

    // Change the selected power-up
    public void SelectPowerup(PowerupData data) {
        selectedPowerup = data;
        selectedImage.sprite = selectedPowerup.image;
    }

    // Add a new button with the specified power-up data
    private void AddButton(PowerupData data) {
        GameObject g = Instantiate(PowerupSelectButton, scrollRect);
        if (data.image != null) // Set the image if power-up has an image
            g.transform.GetChild(0).GetComponent<Image>().sprite = data.image;
        if (data.name != null) // Set the name if power-up has a name
            g.GetComponentInChildren<Text>().text = data.name;
        g.GetComponent<Button>().onClick.AddListener(delegate { SelectPowerup(data); });
    }
}
