using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;                   // this package and all methods/classes used by us are not our own. reference this!!!!


// NOTE: what do we do when more than 1 controller are connected at first, but the 2nd or a controller that wasnt the last one to connect disconnects?
public class PlayerJoinManager : MonoBehaviour {

	public bool PlayerJoinScreenActive = true;   // filler variable for now. Will be replaced when more of UI and menu is implemented.

	public bool debugMode = false;
	public bool player1UsingMouseAndKeyboard = false;

	// NOTE: these 2 variables are now obsolete!
	[Tooltip("Turn this on if you want to load the next scene with the string of its name instead. If you turn this variable on, make sure that the variable nextLevel is filled out in the inspector!")]
	public bool loadNextLevelByName = false;
	[Tooltip("The name of the scene to load when players start a match.")]
	public string nextLevel;

	//both for the references to the UI script -Kyle
	
	public GameObject playersObject;
	private characterSelection selectionOP;

	//reference the mainMenuManager for setting PJActive to true;
	public GameObject mMManager;
	private MainMenuManager currentMenu; 

	// [Tooltip("Reference to the game controller object.")]
	// public GameObject gameManager;
	// public string gameManagerName = "GameController";
	// private ActivePlayerManager playerManager;

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

	// Returns a list/array of player individual status on whether they will play or not. The returned array should not be able to be modified
	// outside of this script (or at least should not have any effect on this script's variables).
	public bool[] GetPLayerReadyStatusList() {
		bool[] roster = new bool[playersReady.Length];
		if (!debugMode) 
			for (int i = 0; i < playersReady.Length; i++) 
				roster[i] = playersReady[i];
		else
			for (int i = 0; i < playersReady.Length; i++)
				roster[i] = true;
		return roster;
	}

	public void DebugActive(bool turnOn) {
		debugMode = turnOn;
	}

	public void KeyboardActive(bool turnOn) {
		player1UsingMouseAndKeyboard = turnOn;
	}

	// void Awake() {
	// 	if (gameManager == null) {
	// 		gameManager = GameObject.Find(gameManagerName);
	// 	}
	// 	if (gameManager != null) 
	// 		playerManager = gameManager.GetComponent<ActivePlayerManager>();
	// 	else
	// 		Debug.Log("The game manager variable was not assigned in the inspector. This will most likely cause errors!");
	// }

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

		//find the script -Kyle
		selectionOP = playersObject.GetComponent<characterSelection>();
		currentMenu = mMManager.GetComponent<MainMenuManager>();
		// if (gameManager != null) 
		// 	playerManager = gameManager.GetComponent<ActivePlayerManager>();
		// else
		// 	Debug.Log("The game manager variable was not assigned in the inspector. This will most likely cause errors!");
	}
	
	// Update is called once per frame
	// NOTE: May want to reimplement Update to function more as a state machine later!!!
	void Update () {
		PlayerJoinScreenActive = (currentMenu.getCurrentPanel() == 2);
		if (PlayerJoinScreenActive) 
		{
			// This loop checks to see which controllers are connected to display confirmation UI element for that controller.
			for (int i = 0; i < 4; i++) {
				// prevStates[i] = currentStates[i];
				// currentStates[i] = GamePad.GetState(players[i]);
				if (i != 0 || !player1UsingMouseAndKeyboard) {
					prevStates[i] = currentStates[i];
					currentStates[i] = GamePad.GetState(players[i]);
					if (currentStates[i].IsConnected) {
						if (!prevStates[i].IsConnected) {
							// FINISH!!!!!
							// This is where we will turn on the correct UI element showing that player to press A.
							// Debug.Log("Player " + players[i] + " controller fis connected.");

							//calling UI -Kyle
							selectionOP.playerIs(i);
							selectionOP.playerConnected();
							
						}
					}
					else if (!currentStates[i].IsConnected /* && prevStates[i].IsConnected*/) {
						// FINISH!!!!!
						// Revert that player's UI section to not connected/empty.
						// Debug.Log("Player " + players[i] + " controller has Disconnected!");
						playersReady[i] = false;
						// if (numOfPlayersReady > 0) numOfPlayersReady--;

						//calling UI -Kyle
						selectionOP.playerIs(i);
						selectionOP.playerDisconnected();
					}
				}
			}

			// This loop checks to see which players have A to confirm they are ready.
			for (int i = 0; i < 4; i++) {
				if (currentStates[i].IsConnected) {
					if (currentStates[i].Buttons.A == ButtonState.Pressed && playersReady[i] == false) {
						// Display the UI element showing the player has confirmed he/she is ready to play.
						playersReady[i] = true;
						Debug.Log("Player " + players[i] + " is ready to play!");

						//calling UI -Kyle
						selectionOP.playerIs(i);
						selectionOP.playerIsReady();
					}
					if ((currentStates[i].Buttons.B == ButtonState.Pressed && playersReady[i] == true) || playersReady[i] == false) {
						playersReady[i] = false;
						Debug.Log("Player " + players[i] + " is NOT ready to play!");

						//calling UI -Kyle
						selectionOP.playerIs(i);
						//selectionOP.playerIsNotReady();
						selectionOP.playerConnected();

					}
				}
			}

			int newNumOfPlayersReady = 0;
			for (int i = 0; i < playersReady.Length; i++) {
				if (playersReady[i] == true)
					newNumOfPlayersReady++;
			}

			// the normal conditions needed for the game to start
			if (!debugMode) 
			{
				RunGameReadyChecksUsingXboxController(newNumOfPlayersReady, 2);

				// // Checks if enough players have confirmed they are ready.
				// if (newNumOfPlayersReady >= 2 && numOfPlayersReady < 2) {
				// 	// Display the UI element showing that the game is ready to start.
				// 	Debug.Log("Game is Ready to start!");
				// 	//calling UI -Kyle
				// 	selectionOP.gameIsReady();
				// }
				// else if (newNumOfPlayersReady < 2 /*&& numOfPlayersReady >= 2*/) {
				// 	// Turn off the UI element showing that the game is ready to start.
				// 	Debug.Log("There are not enough players for the game to start!");
				// 	//calling UI -Kyle
				// 	selectionOP.gameIsNotReady();
				// }

				// // if conditions met, start the match
				// if (numOfPlayersReady >= 2) {
				// 	for (int i = 0; i < playersReady.Length; i++) {
				// 		if (playersReady[i] == true) {
				// 			// If the right conditions are met, this is where the protocol for starting the game happens.
				// 			// NOTE: implementation is subject to change for now.
				// 			if (currentStates[i].Buttons.Start == ButtonState.Pressed && prevStates[i].Buttons.Start == ButtonState.Released) {
				// 				// playerManager.SetPlayersStatus(GetPLayerReadyStatusList());
				// 				if (loadNextLevelByName) 
				// 					SceneManager.LoadScene(nextLevel);
				// 				else 
				// 					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);    // starts the scene used for the first level.
				// 			}
				// 		}
				// 	}
				// }
			}

			// conditions when in debug mode. Debug mode allows the game to start with only 1 player (and maybe more).
			else {
				if (!player1UsingMouseAndKeyboard) {
					RunGameReadyChecksUsingXboxController(newNumOfPlayersReady, 1);

					// // Checks if enough players have confirmed they are ready.
					// if (newNumOfPlayersReady >= 1 /* && numOfPlayersReady < 1*/) {
					// 	// Display the UI element showing that the game is ready to start.
					// 	Debug.Log("Game is Ready to start!");
					// 	//calling UI -Kyle
					// 	selectionOP.gameIsReady();
					// }
					// else if (newNumOfPlayersReady < 1 /* && numOfPlayersReady >= 1 */) {
					// 	// Turn off the UI element showing that the game is ready to start.
					// 	Debug.Log("There are not enough players for the game to start!");
					// 	//calling UI -Kyle
					// 	selectionOP.gameIsNotReady();
					// }

					// // if conditions met, start the match
					// if (numOfPlayersReady >= 1) {
					// 	for (int i = 0; i < playersReady.Length; i++) {
					// 		if (playersReady[i] == true) {
					// 			// If the right conditions are met, this is where the protocol for starting the game happens.
					// 			// NOTE: implementation is subject to change for now.
					// 			if (currentStates[i].Buttons.Start == ButtonState.Pressed && prevStates[i].Buttons.Start == ButtonState.Released) {
					// 				// playerManager.SetPlayersStatus(GetPLayerReadyStatusList());
					// 				if (loadNextLevelByName) 
					// 					SceneManager.LoadScene(nextLevel);
					// 				else 
					// 					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);    // starts the scene used for the first level.
					// 			}
					// 		}
					// 	}
					// }
				}
				else {
					selectionOP.playerIs(0);
					selectionOP.playerIsReady();
					// Checks if enough players have confirmed they are ready.
					// Debug.Log("Game is Ready to start!");
					//calling UI -Kyle
					selectionOP.gameIsReady();

					// if conditions met, start the match
					for (int i = 0; i < playersReady.Length; i++) {
						if (i == 0) {
							if (Input.GetKeyDown(KeyCode.Return)) {
								GameManager.instance.StartGame();
							}
						}
						else {
							if (playersReady[i] == true) {
								// If the right conditions are met, this is where the protocol for starting the game happens.
								// NOTE: implementation is subject to change for now.
								if (currentStates[i].Buttons.Start == ButtonState.Pressed && prevStates[i].Buttons.Start == ButtonState.Released) {
									// playerManager.SetPlayersStatus(GetPLayerReadyStatusList());
									GameManager.instance.StartGame();
								}
							}
						}
					}
				}
			}

			numOfPlayersReady = newNumOfPlayersReady;
		}
	}

	void RunGameReadyChecksUsingXboxController(int newNumOfPlayersReady, int minNumOfPlayers) {
		// Checks if enough players have confirmed they are ready.
				if (newNumOfPlayersReady >= minNumOfPlayers) {
					// Display the UI element showing that the game is ready to start.
					// Debug.Log("Game is Ready to start!");
					//calling UI -Kyle
					selectionOP.gameIsReady();
				}
				else if (newNumOfPlayersReady < minNumOfPlayers /*&& numOfPlayersReady >= 2*/) {
					// Turn off the UI element showing that the game is ready to start.
					// Debug.Log("There are not enough players for the game to start!");
					//calling UI -Kyle
					selectionOP.gameIsNotReady();
				}

				// if conditions met, start the match
				if (numOfPlayersReady >= minNumOfPlayers) {
					for (int i = 0; i < playersReady.Length; i++) {
						if (playersReady[i] == true && currentStates[i].IsConnected) {
							// If the right conditions are met, this is where the protocol for starting the game happens.
							// NOTE: implementation is subject to change for now.
							if (currentStates[i].Buttons.Start == ButtonState.Pressed && prevStates[i].Buttons.Start == ButtonState.Released) {
								// playerManager.SetPlayersStatus(GetPLayerReadyStatusList());
								GameManager.instance.StartGame();
							}
						}
					}
				}
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
