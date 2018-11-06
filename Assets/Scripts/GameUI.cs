using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public PlayerUI PlayerUIPrefab;
    private PlayerUI[] PlayerUIs;
    public Transform PlayerUIParent;

    private void Start() {
        foreach (Player player in GameManager.instance.players) {
            if (player != null) {
                Instantiate<GameObject>(PlayerUIPrefab.gameObject, PlayerUIParent).GetComponent<PlayerUI>().player = player;
            }
        }
    }
}
