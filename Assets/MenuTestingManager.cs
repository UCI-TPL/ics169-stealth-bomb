using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTestingManager : MonoBehaviour {

    public Player[] players;
    public PlayerData DefaultPlayerData;
    [SerializeField]
    private Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start() {
        players = new Player[4];
    }

    public void SetupPlayer(int playerNumber, int controllerNumber, int colorIndex) {
        Debug.Log("Creating player " + playerNumber.ToString() + " with controller " + controllerNumber.ToString());
        if (players[playerNumber] != null) {
            Debug.LogWarning("Player " + playerNumber.ToString() + " has already been created and has not been deleted.");
            return;
        }

        players[playerNumber] = new Player(playerNumber, controllerNumber, DefaultPlayerData, colorIndex);
        players[playerNumber].ResetForRound();
        players[playerNumber].SetController(Instantiate<GameObject>(GameManager.instance.PlayerPrefab.gameObject, spawnPoints[playerNumber].position, Quaternion.identity).GetComponent<PlayerController>());
    }

    public void RemovePlayer(int playerNumber) {
        if (players[playerNumber] == null) {
            Debug.LogWarning("Player " + playerNumber.ToString() + " does not exist or has already been deleted.");
            return;
        }

        players[playerNumber].controller.Destroy();
        players[playerNumber] = null;
    }
}
