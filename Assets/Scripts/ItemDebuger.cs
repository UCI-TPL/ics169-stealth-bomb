using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector3Extensions;

public class ItemDebuger : MonoBehaviour {

    [Tooltip("Prefab for item select button")]
    public GameObject itemSelectButton;
    [Tooltip("Scroll rect that contains item select buttons")]
    public RectTransform scrollRect;
    [Tooltip("Image for current selected item")]
    public Image selectedImage;
    [Tooltip("List of items")]
    public ItemList itemList;
    [Tooltip("Prefab for item container")]
    public GameObject itemContainer;
    private GameObject _itemSpawner;
    private GameObject itemSpawner {
        get {
            if (_itemSpawner != null)
                return _itemSpawner;
            ItemSpawner temp = FindObjectOfType<ItemSpawner>();
            if (temp == null)
                Debug.LogError("Item Spawner not found.");
            return _itemSpawner = temp.gameObject;
        }
    }
    [Tooltip("Item spawner toggle")]
    public Toggle itemToggle;

    private ItemData selectedItem;

    private void Start() {
        SelectItem(itemList.tiers[0].items[0]); // Initalize selected item to first item
        foreach (ItemList.Tier tier in itemList.tiers) {
            foreach (ItemData data in tier.items) {
                AddButton(data); // For each item in the item list create a new selection button
            }
        }
    }

    // Update is called once per frame
    void Update () {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 targetGrid = Vector3.zero;

        RaycastHit hit; // position at mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.GetComponent<Tile>() != null) {
            targetGrid = (hit.point - Tile.TileOffset + hit.normal * 0.1f).Round();
            //DrawCubeWithWire(targetGrid + Tile.TileOffset, 1, Color.white, new Color(1, 1, 1, 0.1f));
            if (Input.GetMouseButtonDown(0)) {
                GameObject g = Instantiate(itemContainer, targetGrid + Tile.TileOffset, Quaternion.identity); // Create a item container
                g.GetComponent<ItemContainer>().SetItemData(selectedItem); // Set the item for the item container
            }
        }
    }

    // Change the selected item
    public void SelectItem(ItemData data) {
        selectedItem = data;
        selectedImage.sprite = selectedItem.image;
    }

    // Add a new button with the specified item data
    private void AddButton(ItemData data) {
        GameObject g = Instantiate(itemSelectButton, scrollRect);
        if (data.image != null) // Set the image if item has an image
            g.transform.GetChild(0).GetComponent<Image>().sprite = data.image;
        if (data.name != null) // Set the name if item has a name
            g.GetComponentInChildren<Text>().text = data.name;
        g.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(data); });
    }

    public void ToggleItemSpawner() {
        itemSpawner.SetActive(itemToggle.isOn);
    }
}
