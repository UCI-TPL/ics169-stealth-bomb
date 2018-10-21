using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;                   // this package and all methods/classes used by us are not our own. reference this!!!!


// NOTE: what do we do when more than 1 controller are connected at first, but the 2nd or a controller that wasnt the last one to connect disconnects?
public class PlayerJoinManager : MonoBehaviour {

	public bool PlayerJoinScreenActive = true;   // filler variable for now. Will be replaced when more of UI and menu is implemented.

	bool[] playersReady;
	// public bool player1Ready;
	// public bool player2Ready;
	// public bool player3Ready;
	// public bool player4Ready;

	// PlayerIndex player1 = PlayerIndex.One;
	// PlayerIndex player2 = PlayerIndex.Two;
	// PlayerIndex player3 = PlayerIndex.Three;
	// PlayerIndex player4 = PlayerIndex.Four;

	PlayerIndex[] players;
	GamePadState[] currentStates;
	GamePadState[] prevStates;

	private int numOfPlayersReady;

	// Use this for initialization
	void Start () {
		players = new PlayerIndex[4];
		currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		playersReady = new bool[4];
		for (int i = 0; i < playersReady.Length; i++) {
			players[i] = (PlayerIndex) i;
			playersReady[i] = false;
		}

		// player1Ready = false;
		// player2Ready = false;
		// player3Ready = false;
		// player4Ready = false;
		numOfPlayersReady = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerJoinScreenActive) 
		{
			// This loop checks to see which controllers are connected to display confirmation UI element for that controller.
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState(players[i]);;
				if (currentStates[i].IsConnected) {
					if (!prevStates[i].IsConnected) {
						// FINISH!!!!!
						// This is where we will turn on the correct UI element showing that player to press A.
						Debug.Log("Player " + players[i] + " controller is connected.");
					}
				}
				else if (!currentStates[i].IsConnected && prevStates[i].IsConnected) {
					// FINISH!!!!!
					// Revert that player's UI section to not connected/empty.
					Debug.Log("Player " + players[i] + " controller has Disconnected!");
					playersReady[i] = false;
					// if (numOfPlayersReady > 0) numOfPlayersReady--;
				}
			}

			// This loop checks to see which players have A to confirm they are ready.
			for (int i = 0; i < 4; i++) {
				if (currentStates[i].IsConnected) {
					if (currentStates[i].Buttons.A == ButtonState.Pressed && playersReady[i] == false) {
						// FINISH!!!!!
						// Display the UI element showing the player has confirmed he/she is ready to play.
						playersReady[i] = true;
						Debug.Log("Player " + players[i] + " is ready to play!");
					}
					if (currentStates[i].Buttons.B == ButtonState.Pressed && playersReady[i] == true) {
						playersReady[i] = false;
						Debug.Log("Player " + players[i] + " is NOT ready to play!");
					}
				}
			}

			int newNumOfPlayersReady = 0;
			for (int i = 0; i < playersReady.Length; i++) {
				if (playersReady[i] == true)
					newNumOfPlayersReady++;
			}

			// Checks if enough players have confirmed they are ready.
			if (newNumOfPlayersReady >= 2 && numOfPlayersReady < 2) {
				// FINISH!!!!!
				// Display the UI element showing that the game is ready to start.
				Debug.Log("Game is Ready to start!");
			}
			else if (newNumOfPlayersReady < 2 && numOfPlayersReady >= 2) {
				// FINISH!!!!!
				// Turn off the UI element showing that the game is ready to start.
				Debug.Log("There are not enough players for the game to start!");
			}

			numOfPlayersReady = newNumOfPlayersReady;
		}

		/* Old, Outdated code */
		// string[] connectedControllers = Input.GetJoystickNames();
		// Debug.Log("Number of controllers connected: " + connectedControllers.Length);
		// for (int i = 1; i <= connectedControllers.Length; i++) {
		// 	//Debug.Log("controller #" + i + ": " + connectedControllers[i-1]);
		// 	if (IsPlayerControllerConnected(i) && playersReady[i-1] == false) {
		// 		//ThenShowPlayerControllerConnectedInUIforPlayerI();            // placeholder! remove later!
		// 		if (Input.GetButtonDown("P" + i + "_A")) {
		// 			playersReady[i-1] = true;
		// 		}
		// 	}
		// }
	}






	/**** ALL HELPER FUNCTIONS BELOW THIS POINT ARE NOW OUT OF DATE AND DO NOT WORK!!!!  ****/
	/* If any of the team needs need new helper functions, let David know on Discord. */


	// public int NumOfControllersConnected() {
	// 	return Input.GetJoystickNames().Length;
	// }

	// /* Based on the parameter given, it returns a boolean that represents whether a specific controller is connected.
	//  * If zero players are connected, the method will always return False. If the parameter is more than 4, then the 
	//  * method will also return False since there can only be up to 4 players.
    //  * Parameter: player = (type int) represents the integer of the player. ex: player 1 should be represented by 1.
	//  */
	// public bool IsPlayerControllerConnected(int player) {
	// 	if (player <= 0) {
	// 		return false;
	// 	}
	// 	else if (player >= 1 && player <= 4) {
	// 		return Input.GetJoystickNames().Length >= player;
	// 	}
	// 	else {      // for more than 4 players
	// 		return false;
	// 	}
	// }

	// public bool IsPlayer1ControllerConnected() {
	// 	return Input.GetJoystickNames().Length >= 1;
	// }

	// public bool IsPlayer2ControllerConnected() {
	// 	return Input.GetJoystickNames().Length >= 2;
	// }

	// public bool IsPlayer3ControllerConnected() {
	// 	return Input.GetJoystickNames().Length >= 3;
	// }

	// public bool IsPlayer4ControllerConnected() {
	// 	return Input.GetJoystickNames().Length >= 4;
	// }

	// public void ConfirmPlayerReady(int player) {
	// 	switch (player) {
	// 		case 1: 
	// 			player1Ready = true;
	// 			numOfPlayersReady++;
	// 			break;
	// 		case 2:
	// 			player2Ready = true;
	// 			numOfPlayersReady++;
	// 			break;
	// 		case 3:
	// 			player3Ready = true;
	// 			numOfPlayersReady++;
	// 			break;
	// 		case 4:
	// 			player4Ready = true;
	// 			numOfPlayersReady++;
	// 			break;
	// 		default:
	// 			Debug.Log("There should only be 1 to 4 players!");
	// 			break;
	// 	}
	// }
}
