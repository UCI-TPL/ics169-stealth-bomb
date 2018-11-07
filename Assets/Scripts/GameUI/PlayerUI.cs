using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public Text rankDisplay;
    public Slider experianceSlider;
    public GameObject Crown;
    public PowerupShelfUI powerUpShelf;
    public Player player;

    private void Start() {
        player.onAddPowerUp += powerUpShelf.AddPowerup;
    }

    private void Update() {
        rankDisplay.text = player.rank.ToString();
        experianceSlider.value = player.experiance - Mathf.Floor(player.experiance);
        Crown.SetActive(player == GameManager.instance.leader);
    }
}
