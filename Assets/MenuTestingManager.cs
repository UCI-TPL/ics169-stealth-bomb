using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTestingManager : MonoBehaviour {

    public Player[] players;
    public PlayerData DefaultPlayerData;
    [SerializeField]
    private GameObject[] cameras;
    [SerializeField]
    private Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start() {
        Color[] colorArray = new Color[] { new Color(0, 0, 0, 1) };
        Texture2D t = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        t.SetPixels(colorArray);
        t.Apply();
        Shader.SetGlobalTexture(Shader.PropertyToID("_TileDamageMap"), t);
        Shader.SetGlobalVector(Shader.PropertyToID("_TileMapSize"), Vector3.one);
        players = new Player[4];
        foreach (GameObject g in cameras)
            g.SetActive(false);
    }

    public void SetupPlayer(int playerNumber, int controllerNumber, int colorIndex) {
        Debug.Log("Creating player " + playerNumber.ToString() + " with controller " + controllerNumber.ToString());
        if (players[playerNumber] != null) {
            Debug.LogWarning("Player " + playerNumber.ToString() + " has already been created and has not been deleted.");
            return;
        }
        cameras[playerNumber].SetActive(true);
        players[playerNumber] = new Player(playerNumber, controllerNumber, DefaultPlayerData, colorIndex);
        players[playerNumber].ResetForRound();
        players[playerNumber].SetController(Instantiate<GameObject>(GameManager.instance.PlayerPrefab.gameObject, spawnPoints[playerNumber].position, Quaternion.identity).GetComponent<PlayerController>());
    }

    public void RemovePlayer(int playerNumber) {
        if (players[playerNumber] == null) {
            Debug.LogWarning("Player " + playerNumber.ToString() + " does not exist or has already been deleted.");
            return;
        }
        cameras[playerNumber].SetActive(false);
        players[playerNumber].controller.Destroy();
        players[playerNumber] = null;
    }
}
