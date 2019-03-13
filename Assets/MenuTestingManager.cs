using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTestingManager : MonoBehaviour {

    private Player[] players;
    public PlayerData DefaultPlayerData;
    [SerializeField]
    private Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start() {
        players = SetupPlayers(4);

        for (int i = 0; i < players.Length; ++i) {
            players[i].ResetForRound();
            players[i].SetController(Instantiate<GameObject>(GameManager.instance.PlayerPrefab.gameObject, spawnPoints[i].position, Quaternion.identity).GetComponent<PlayerController>());
        }
    }

    private Player[] SetupPlayers(int count) {
        Player[] result = new Player[count];
        for (int i = 0; i < count; ++i) {
            result[i] = new Player(i, i, DefaultPlayerData);
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
