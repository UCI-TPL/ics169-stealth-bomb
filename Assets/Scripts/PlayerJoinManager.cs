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
	public bool player1Ready;
	public bool player2Ready;
	public bool player3Ready;
	public bool player4Ready;

	private int numOfPlayersReady;

	// Use this for initialization
	void Start () {
		playersReady = new bool[4];
		for (int i = 0; i < playersReady.Length; i++) {
			playersReady[i] = false;
		}

		player1Ready = false;
		player2Ready = false;
		player3Ready = false;
		player4Ready = false;
		numOfPlayersReady = 0;
	}
	
	// Update is called once per frame
	void Update () {
		string[] connectedControllers = Input.GetJoystickNames();
		Debug.Log("Number of controllers connected: " + connectedControllers.Length);
		for (int i = 1; i <= connectedControllers.Length; i++) {
			//Debug.Log("controller #" + i + ": " + connectedControllers[i-1]);
			if (IsPlayerControllerConnected(i) && playersReady[i-1] == false) {
				//ThenShowPlayerControllerConnectedInUIforPlayerI();            // placeholder! remove later!
				if (Input.GetButtonDown("P" + i + "_A")) {
					playersReady[i-1] = true;
				}
			}
		}

		for (int i = 0; i < playersReady.Length; i++) {
			if (playersReady[i] == true)
				numOfPlayersReady++;
		}

		if (numOfPlayersReady >= 2) {
			//SetStartGameUIobjectToTrue();         // placeholder! remove later!
		}

		// NOTE: should have a case for when a controller disconnects.
	}

	public int NumOfControllersConnected() {
		return Input.GetJoystickNames().Length;
	}

	/* Based on the parameter given, it returns a boolean that represents whether a specific controller is connected.
	 * If zero players are connected, the method will always return False. If the parameter is more than 4, then the 
	 * method will also return False since there can only be up to 4 players.
     * Parameter: player = (type int) represents the integer of the player. ex: player 1 should be represented by 1.
	 */
	public bool IsPlayerControllerConnected(int player) {
		if (player <= 0) {
			return false;
		}
		else if (player >= 1 && player <= 4) {
			return Input.GetJoystickNames().Length >= player;
		}
		else {      // for more than 4 players
			return false;
		}
	}

	public bool IsPlayer1ControllerConnected() {
		return Input.GetJoystickNames().Length >= 1;
	}

	public bool IsPlayer2ControllerConnected() {
		return Input.GetJoystickNames().Length >= 2;
	}

	public bool IsPlayer3ControllerConnected() {
		return Input.GetJoystickNames().Length >= 3;
	}

	public bool IsPlayer4ControllerConnected() {
		return Input.GetJoystickNames().Length >= 4;
	}

	public void ConfirmPlayerReady(int player) {
		switch (player) {
			case 1: 
				player1Ready = true;
				numOfPlayersReady++;
				break;
			case 2:
				player2Ready = true;
				numOfPlayersReady++;
				break;
			case 3:
				player3Ready = true;
				numOfPlayersReady++;
				break;
			case 4:
				player4Ready = true;
				numOfPlayersReady++;
				break;
			default:
				Debug.Log("There should only be 1 to 4 players!");
				break;
		}
	}
}
