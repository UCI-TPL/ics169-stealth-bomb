using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivePlayerManager : MonoBehaviour {

	public static ActivePlayerManager instance;
	
	private Scene scene;

	// array of possible players. idx=0 is player 1. idx=1 is player 2. idx=2 is player 3. idx=3 is player 4.
	private bool[] playersInGame;
	private int numOfPlayers;

	void Awake() {
		// Borrowed from GameController.cs
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		playersInGame = new bool[0];
		numOfPlayers = 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// scene = SceneManager.GetActiveScene();

		// // players are spawned here
		// if (!scene.name.Equals("mainMenu")) {}
	}

	// Call this method to set up which players will be playing. 
	// This should only be called in the main menu by PlayerJoinManager.cs
	// In the parameter, each index represents a player: false means not playing, true means playing.
	public void SetPlayersStatus(bool[] statusArray) {
		if (statusArray.Length == 4) 
		{
			playersInGame = statusArray;
			int newNumOfPlayers = 0;
			for (int i = 0; i < playersInGame.Length; i++) 
				if (playersInGame[i] == true)
					newNumOfPlayers++;
			
			numOfPlayers = newNumOfPlayers;
		}
	}

	/* Getters */

	/* Call this method to get the status of players. 
		The returned array of bools represents who is playing. The array is always 4 in length since the max is 4 players.
	 */
	public bool[] GetPlayersStatus() {
		bool[] copy = new bool[playersInGame.Length];
		for (int i = 0; i < playersInGame.Length; i++) 
			copy[i] = playersInGame[i];
		return copy;
	}

	public int GetNumberOfPlayers() {
		return numOfPlayers;
	}

	public bool isPlayerInGame(int player) {
		return playersInGame[player - 1];
	}

	public bool IsPlayer1Playing() {
		return playersInGame[0];
	}

	public bool IsPlayer2Playing() {
		return playersInGame[1];
	}

	public bool IsPlayer3Playing() {
		return playersInGame[2];
	}

	public bool IsPlayer4Playing() {
		return playersInGame[3];
	}

	/* private helper methods */
	// private bool[] DeepCopy(bool[] original) {}
}
