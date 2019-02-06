using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;                   // this package and all methods/classes used by us are not our own. reference this!!!!


// NOTE: what do we do when more than 1 controller are connected at first, but the 2nd or a controller that wasnt the last one to connect disconnects?
public class PlayerJoinManager : MonoBehaviour {

	public bool usingNewPlayerJoinSystem = false;

	[HideInInspector]
	public bool PlayerJoinScreenActive = true;   // filler variable for now. Will be replaced when more of UI and menu is implemented.

	public bool debugMode = false;
	public bool playerUsingMouseAndKeyboard = false;
	public float cooldown = 10.0f;

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

	public RectTransform[] joinPrompts;
	public RectTransform[] playersUI;

	// [Tooltip("Reference to the game controller object.")]
	// public GameObject gameManager;
	// public string gameManagerName = "GameController";
	// private ActivePlayerManager playerManager;

	bool[] playersJoined;
	bool[] playersReady;
	bool[] controllersConnected;
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
	float[] playerTimers;
	float inputTimer;

	private int numOfPlayersJoined;
	private int playerUsingKeyboardIdx;    // represents the player index that is currently using MMK input. if value isnt between 0-3, then no one is using MMK.

	private InputManager input;

	private bool[] playersControlsGuideActive;   // temp private variable to represent which player has the controller guide showing while the real UI implementation isnt ready.

	private float player1X = -300;
	private float player2X = -100;
	private float player3X = 100;
	private float player4X = 300;

	private float fourPlayersWidth = 1000;
	private float fourPlayersHeight = 1500;

	private int[] defaultInputControllerNumbers;

	// Returns a list/array of player individual status on whether they will play or not. The returned array should not be able to be modified
	// outside of this script (or at least should not have any effect on this script's variables).
	public bool[] GetPLayerReadyStatusList() {
		bool[] roster = new bool[playersJoined.Length];
		if (!debugMode) 
			for (int i = 0; i < playersJoined.Length; i++) 
				roster[i] = playersJoined[i];
		else
			for (int i = 0; i < playersJoined.Length; i++)
				roster[i] = true;
		return roster;
	}

	public void DebugActive(bool turnOn) {
		debugMode = turnOn;
	}

	public void KeyboardActive(bool turnOn) {
		playerUsingMouseAndKeyboard = turnOn;
		InputManager.inputManager.UseMouseAndKeyboardForFirstDisconnectedPlayer(turnOn);
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

	private void Awake() {
		//find the script -Kyle
		if (!usingNewPlayerJoinSystem)
			selectionOP = playersObject.GetComponent<characterSelection>();
		
		currentMenu = mMManager.GetComponent<MainMenuManager>();
	}

	// Use this for initialization
	private void Start () {
		input = InputManager.inputManager;
		players = new PlayerIndex[4];
		currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		playerTimers = new float[4];
		playersJoined = new bool[4];
		playersReady = new bool[4];
		controllersConnected = new bool[4];
		defaultInputControllerNumbers = new int[] { 0, 1, 2, 3 };
		playersControlsGuideActive = new bool[4];

		for (int i = 0; i < playersJoined.Length; i++) {
			// Debug.Log("playersReady index=" + i);
			// InputManager.inputManager.controllers[i].confirm.OnDown.AddListener( () => ReadyPlayer(i) );
			// input.controllers[i].confirm.OnDown.AddListener( delegate { ReadyPlayer(0); } );
			players[i] = (PlayerIndex) i;
			playersJoined[i] = false;
			playersReady[i] = false;
			controllersConnected[i] = false;
			playersControlsGuideActive[i] = false;
			playerTimers[i] = 0.0f;
			AssignControllerEvents(i);
			if (usingNewPlayerJoinSystem) {
				ResetPlayer(i);
			}
			// InputManager.inputManager.controllers[i].confirm.OnDown.AddListener(TestConfirm);
			// InputManager.inputManager.controllers[i].confirm.OnDown.AddListener( () => ReadyPlayer(i) );
			// InputManager.inputManager.controllers[i].cancel.OnDown.AddListener();
		}

		// player1Ready = false;
		// player2Ready = false;
		// player3Ready = false;
		// player4Ready = false;
		numOfPlayersJoined = 0;
		playerUsingKeyboardIdx = -1;
		inputTimer = 0.0f;
		// if (gameManager != null) 
		// 	playerManager = gameManager.GetComponent<ActivePlayerManager>();
		// else
		// 	Debug.Log("The game manager variable was not assigned in the inspector. This will most likely cause errors!");
	}
	
	// Update is called once per frame
	// NOTE: May want to reimplement Update to function more as a state machine later!!!
	void Update () {
		if (!usingNewPlayerJoinSystem) {
			UpdateOldVersion();
		}
		else {
			UpdateNewVersion();
		}
	}


	///// Helper Methods /////


	// Callback helper methods for player input //

	// helper callback method that readies the player specified by the parameter/index.
	private void PlayerJoin(int playerIdx) {
		// Debug.Log("ReadyPlayer called for player " + playerIdx + ".");
		if (CanPlayerPressButton(playerIdx) && playersJoined[playerIdx] == false) {
			// Display the UI element showing the player has confirmed he/she is ready to play.
			playersJoined[playerIdx] = true;
			// Debug.Log("Player " + players[i] + " is ready to play!");

			if (!usingNewPlayerJoinSystem) {
				//calling UI -Kyle
				selectionOP.playerIs(playerIdx);
				selectionOP.playerIsReady();
			}
			else {
				joinPrompts[playerIdx].gameObject.SetActive(false);
				playersUI[playerIdx].gameObject.SetActive(true);
			}
		}
	}

	// helper callback method that unreadies the player specified by the parameter/index, or returns game to main menu panel.
	private void SetPlayerBack(int playerIdx) {
		if (CanPlayerPressButton(playerIdx)) {
			if (playersJoined[playerIdx] == true) {
				if (usingNewPlayerJoinSystem) {
					// if player has already pressed start (readied up), then it will take the player back to previous stage
					if (playersReady[playerIdx] == true) {
						playersReady[playerIdx] = false;
						playersUI[playerIdx].gameObject.SetActive(true);
					}
					// otherwise, player will be unjoin the lobby and will have to press A again to rejoin.
					else {
						// TEST THIS!!!!!
						playersJoined[playerIdx] = false;
						playersUI[playerIdx].gameObject.SetActive(false);
						joinPrompts[playerIdx].gameObject.SetActive(false);
					}
				}
				else {
					playersJoined[playerIdx] = false;
					// Debug.Log("Player " + players[i] + " is NOT ready to play!");

					//calling UI -Kyle
					selectionOP.playerIs(playerIdx);
					//selectionOP.playerIsNotReady();
					selectionOP.playerConnected();
				}

				// joinPrompts[playerIdx].gameObject.SetActive(true);
			}

			else {
				for (int idx = 0; idx < playersJoined.Length; idx++) {
					ResetPlayer(idx);
				}
				currentMenu.setMenu(1);
			}
		}
	}

	// helper callback method that toggles on or off the specified player's personal controller guide on their player box.
	private void ShowPlayerControls(int playerIdx) {
		// player needs to join in order to see their controls
		if (CanPlayerPressButton(playerIdx) && playersJoined[playerIdx]) {
			if (!playersControlsGuideActive[playerIdx]) {
				Debug.Log("Player " + (playerIdx + 1) + " turned on their controller guide.");
			}
			else {
				Debug.Log("Player " + (playerIdx + 1) + " turned off their controller guide.");
			}

			// temp toggle on or off for controller guide
			playersControlsGuideActive[playerIdx] = !playersControlsGuideActive[playerIdx];
		}
	}

	// helper callback method that starts the game if the player and others are ready (old update() version).
	private void StartGame(int playerIdx) {
		if (CanPlayerPressButton(playerIdx) && playersJoined[playerIdx] == true) {
			if ((!debugMode && numOfPlayersJoined >= 2) || (debugMode && numOfPlayersJoined >= 1))
				GameManager.instance.StartGame(GetPLayerReadyStatusList(), defaultInputControllerNumbers);
		}
	}


	// General Helper Methods //

	// helper method that returns whether or not the specified player can enter input.
	private bool CanPlayerPressButton(int playerIdx) {
		return currentStates[playerIdx].IsConnected && PlayerJoinScreenActive && inputTimer >= cooldown;
	}


	// helper method that assigns what functions should be called by what input/event.
	private void AssignControllerEvents(int playerIdx) {
		// A button, B button, and Start button has been assigned. Still need to assign Y button.
		switch (playerIdx) {
			case 0:
				input.controllers[playerIdx].confirm.OnDown.AddListener( () => PlayerJoin(0) );
				input.controllers[playerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(0) );
				input.controllers[playerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(0) );
				input.controllers[playerIdx].start.OnDown.AddListener( () => StartGame(0) );
				break;
			case 1:
				input.controllers[playerIdx].confirm.OnDown.AddListener( () => PlayerJoin(1) );
				input.controllers[playerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(1) );
				input.controllers[playerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(1) );
				input.controllers[playerIdx].start.OnDown.AddListener( () => StartGame(1) );
				break;
			case 2:
				input.controllers[playerIdx].confirm.OnDown.AddListener( () => PlayerJoin(2) );
				input.controllers[playerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(2) );
				input.controllers[playerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(2) );
				input.controllers[playerIdx].start.OnDown.AddListener( () => StartGame(2) );
				break;
			case 3:
				input.controllers[playerIdx].confirm.OnDown.AddListener( () => PlayerJoin(3) );
				input.controllers[playerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(3) );
				input.controllers[playerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(3) );
				input.controllers[playerIdx].start.OnDown.AddListener( () => StartGame(3) );
				break;
			default:
				Debug.Log("variable playerIdx out of range!");
				break;
		}
	}

	private void TestConfirm() {
		Debug.Log("button A pressed.");
	}

	// Helper function that checks if players can start the game and, if so, adds UI to indicate this state.
	// Also, still manages taking in MMK input since input manager's MMK support has not been integrated into the join manager yet
	void RunGameReadyChecks(int newNumOfPlayersReady, int minNumOfPlayers) {
		// Checks if enough players have confirmed they are ready.
		if (newNumOfPlayersReady >= minNumOfPlayers) {
			// Display the UI element showing that the game is ready to start.
			// Debug.Log("Game is Ready to start!");
			if (!usingNewPlayerJoinSystem) {
				//calling UI -Kyle
				selectionOP.gameIsReady();
			}
		}
		else if (newNumOfPlayersReady < minNumOfPlayers /*&& numOfPlayersReady >= 2*/) {
			// Turn off the UI element showing that the game is ready to start.
			// Debug.Log("There are not enough players for the game to start!");
			if (!usingNewPlayerJoinSystem) {
				//calling UI -Kyle
				selectionOP.gameIsNotReady();
			}
		}

		// if conditions met, start the match
		if (!usingNewPlayerJoinSystem) {
			if (numOfPlayersJoined >= minNumOfPlayers) {
				// if conditions met, start the match
				for (int i = 0; i < playersJoined.Length; i++) {
					if (i == playerUsingKeyboardIdx && playerUsingMouseAndKeyboard) {
						if (Input.GetKeyDown(KeyCode.Return)) {
							GameManager.instance.StartGame(GetPLayerReadyStatusList(), defaultInputControllerNumbers);
						}
					}
					else {
						// if (playersReady[i] == true) {
						// 	// If the right conditions are met, this is where the protocol for starting the game happens.
						// 	// NOTE: implementation is subject to change for now.
						// 	if (currentStates[i].Buttons.Start == ButtonState.Pressed && prevStates[i].Buttons.Start == ButtonState.Released) {
						// 		// playerManager.SetPlayersStatus(GetPLayerReadyStatusList());
						// 		GameManager.instance.StartGame(GetPLayerReadyStatusList());
						// 	}
						// }
					}
				}
			}
		}
	}


	private void UpdateNewVersion() {
		PlayerJoinScreenActive = (currentMenu.getCurrentPanel() == 2);
		// Debug.Log("playerUsingMouseAndKeyboard: " + playerUsingMouseAndKeyboard);
		if (PlayerJoinScreenActive) 
		{
			// checks to see if a player is using mouse and keyboard.
			if (playerUsingMouseAndKeyboard) {
				for (int i = 0; i <= playersJoined.Length; i++) {
					if (i == playersJoined.Length) {
						playerUsingKeyboardIdx = -1;
						break;
					}
					if (InputManager.inputManager.controllers[i].type == InputManager.Controller.Type.MouseKeyboard) {
						playerUsingKeyboardIdx = i;
						break;
					}
				}
			}

			// Debug.Log("playerUsingKeyboardIdx = " + playerUsingKeyboardIdx);

			// This loop checks to see which controllers are connected to display confirmation UI element for that controller.
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState(players[i]);
				if (currentStates[i].IsConnected || i != playerUsingKeyboardIdx || !playerUsingMouseAndKeyboard) {
					if (currentStates[i].IsConnected) {
						if (!prevStates[i].IsConnected) {
							controllersConnected[i] = true;
							playersJoined[i] = false;
							
						}
					}
					else if (!currentStates[i].IsConnected /* && prevStates[i].IsConnected*/) {
						controllersConnected[i] = false;
						playersJoined[i] = false;
					}
				}
				// condition for if player is using mouse and keyboard
				else {
					selectionOP.playerIs(i);
					selectionOP.playerIsReady();
					playersJoined[i] = true;
				}
			}

			// counts the number of connected controllers
			int numOfControllersConnected = 0;
			for (int i = 0; i < controllersConnected.Length; i++) {
				if (controllersConnected[i]) {
					numOfControllersConnected++;
				}
			}

			// turns on join prompts for connected controllers
			for (int i = 0; i < controllersConnected.Length; i++) {
				if (i < numOfControllersConnected && !playersJoined[i]) {
					joinPrompts[i].gameObject.SetActive(true);
					playersUI[i].gameObject.SetActive(false);
					if (i == 0) {
						if (numOfControllersConnected < 3) 
							joinPrompts[i].transform.localPosition = new Vector3(-100.0f, 0.0f, 0.0f);
						else
							joinPrompts[i].transform.localPosition = new Vector3(-300.0f, 0.0f, 0.0f);
					}
					else {
						joinPrompts[i].transform.localPosition = new Vector3(joinPrompts[i-1].transform.localPosition.x + 200.0f, 0.0f, 0.0f);
					}
					playersUI[i].transform.localPosition = joinPrompts[i].transform.localPosition;
				}
				else {
					joinPrompts[i].gameObject.SetActive(false);
				}
			}

			// This loop checks to see which players have A to confirm they are ready.
			for (int i = 0; i < 4; i++) {
				// Debug.Log("Player " + (i + 1) + " Timer = " + playerTimers[i]);
				if (currentStates[i].IsConnected && playerTimers[i] >= cooldown) {
					// sets players back to main menu and resets their ready status.
					if (currentStates[i].Buttons.B == ButtonState.Pressed && playersJoined[i] == false) {
						
					}
					else if (/*(currentStates[i].Buttons.B == ButtonState.Pressed && playersReady[i] == true) ||*/ playersJoined[i] == false) {
						playersJoined[i] = false;
						// Debug.Log("Player " + players[i] + " is NOT ready to play!");

						playersUI[i].gameObject.SetActive(false);
						joinPrompts[i].gameObject.SetActive(true);
						// //calling UI -Kyle
						// selectionOP.playerIs(i);
						// //selectionOP.playerIsNotReady();
						// selectionOP.playerConnected();
					}

					// playerTimers[i] = 0.0f;
					playerTimers[i] += 1.0f * Time.deltaTime;
				}
				else {
					playerTimers[i] += 1.0f * Time.deltaTime;
				}
			}

			// Debug.Log("players ready: " + playersReady[0] + ", " + playersReady[1] + ", " + playersReady[2] + ", " + playersReady[3]);

			int newNumOfPlayersReady = 0;
			for (int i = 0; i < playersJoined.Length; i++) {
				if (playersJoined[i] == true)
					newNumOfPlayersReady++;
			}

			// the normal conditions needed for the game to start
			if (!debugMode) 
			{
				RunGameReadyChecks(newNumOfPlayersReady, 2);
			}

			// conditions when in debug mode. Debug mode allows the game to start with only 1 player (and maybe more).
			else {
				RunGameReadyChecks(newNumOfPlayersReady, 1);
			}

			numOfPlayersJoined = newNumOfPlayersReady;
			inputTimer += 1.0f * Time.deltaTime;
		}

		else {
			inputTimer = 0.0f;
		}
	}

	// Obsolete/irelevant: Dont use this!!!!!!
	private void SpaceOutPlayerUI() {
		switch (numOfPlayersJoined) {
			case 1:
				for (int i = 0; i < playersUI.Length; i++) {
					if (i == 0) {
						playersUI[i].gameObject.SetActive(true);
						playersUI[i].transform.localPosition = new Vector3(0.0f, playersUI[1].transform.localPosition.y, playersUI[1].transform.localPosition.z);
					}
					else {
						playersUI[i].gameObject.SetActive(false);
					}
				}
				playersUI[1].gameObject.SetActive(true);
				playersUI[1].transform.localPosition = new Vector3(0.0f, playersUI[1].transform.localPosition.y, playersUI[1].transform.localPosition.z);
				break;
		}
	}

	// helper method that resets the timer for controller input (old)
	void ResetTimer(int playerIdx) {
		playerTimers[playerIdx] = 0.0f;
	}

	// helper method that resets the player join UI for the player specified by the parameter
	void ResetPlayer(int playerIdx) {
		playersJoined[playerIdx] = false;
		if (!usingNewPlayerJoinSystem) {
			selectionOP.playerIs(playerIdx);
			selectionOP.playerConnected();
		}
		else {
			playersReady[playerIdx] = false;
			joinPrompts[playerIdx].gameObject.SetActive(false);
			playersUI[playerIdx].gameObject.SetActive(false);
		}
	}


	private void UpdateOldVersion() {
		PlayerJoinScreenActive = (currentMenu.getCurrentPanel() == 2);
		// Debug.Log("playerUsingMouseAndKeyboard: " + playerUsingMouseAndKeyboard);
		if (PlayerJoinScreenActive) 
		{
			// checks to see if a player is using mouse and keyboard.
			if (playerUsingMouseAndKeyboard) {
				for (int i = 0; i <= playersJoined.Length; i++) {
					if (i == playersJoined.Length) {
						playerUsingKeyboardIdx = -1;
						break;
					}
					if (InputManager.inputManager.controllers[i].type == InputManager.Controller.Type.MouseKeyboard) {
						playerUsingKeyboardIdx = i;
						break;
					}
				}
			}

			// Debug.Log("playerUsingKeyboardIdx = " + playerUsingKeyboardIdx);

			// This loop checks to see which controllers are connected to display confirmation UI element for that controller.
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState(players[i]);
				if (currentStates[i].IsConnected || i != playerUsingKeyboardIdx || !playerUsingMouseAndKeyboard) {
					// prevStates[i] = currentStates[i];
					// currentStates[i] = GamePad.GetState(players[i]);
					if (currentStates[i].IsConnected) {
						if (!prevStates[i].IsConnected) {
							// This is where we will turn on the correct UI element showing that player to press A.
							// Debug.Log("Player " + players[i] + " controller fis connected.");
							//calling UI -Kyle
							selectionOP.playerIs(i);
							selectionOP.playerConnected();

							// test this line later!!!
							playersJoined[i] = false;
							
						}
					}
					else if (!currentStates[i].IsConnected /* && prevStates[i].IsConnected*/) {
						// Revert that player's UI section to not connected/empty.
						// Debug.Log("Player " + players[i] + " controller has Disconnected!");
						playersJoined[i] = false;
						// if (numOfPlayersReady > 0) numOfPlayersReady--;

						//calling UI -Kyle
						selectionOP.playerIs(i);
						selectionOP.playerDisconnected();
					}
				}
				// condition for if player is using mouse and keyboard
				else {
					selectionOP.playerIs(i);
					selectionOP.playerIsReady();
					playersJoined[i] = true;
				}
			}

			// This loop checks to see which players have A to confirm they are ready.
			for (int i = 0; i < 4; i++) {
				// Debug.Log("Player " + (i + 1) + " Timer = " + playerTimers[i]);
				if (currentStates[i].IsConnected && playerTimers[i] >= cooldown) {
					/*if (currentStates[i].Buttons.A == ButtonState.Pressed && playersReady[i] == false) {
						// Display the UI element showing the player has confirmed he/she is ready to play.
						playersReady[i] = true;
						// Debug.Log("Player " + players[i] + " is ready to play!");

						//calling UI -Kyle
						selectionOP.playerIs(i);
						selectionOP.playerIsReady();
						ResetTimer(i);
					}*/
					// sets players back to main menu and resets their ready status.
					if (currentStates[i].Buttons.B == ButtonState.Pressed && playersJoined[i] == false) {
						// ResetTimer(i);
						// for (int idx = 0; idx < playersReady.Length; idx++) {
						// 	ResetPlayer(idx);
						// }
						// currentMenu.setMenu(1);
					}
					else if (/*(currentStates[i].Buttons.B == ButtonState.Pressed && playersReady[i] == true) ||*/ playersJoined[i] == false) {
						// if (currentStates[i].Buttons.B == ButtonState.Pressed) 
						// 	ResetTimer(i);
						
						playersJoined[i] = false;
						// Debug.Log("Player " + players[i] + " is NOT ready to play!");

						//calling UI -Kyle
						selectionOP.playerIs(i);
						//selectionOP.playerIsNotReady();
						selectionOP.playerConnected();
					}

					// playerTimers[i] = 0.0f;
					playerTimers[i] += 1.0f * Time.deltaTime;
				}
				else {
					playerTimers[i] += 1.0f * Time.deltaTime;
				}
			}

			// Debug.Log("players ready: " + playersReady[0] + ", " + playersReady[1] + ", " + playersReady[2] + ", " + playersReady[3]);

			int newNumOfPlayersReady = 0;
			for (int i = 0; i < playersJoined.Length; i++) {
				if (playersJoined[i] == true)
					newNumOfPlayersReady++;
			}

			// the normal conditions needed for the game to start
			if (!debugMode) 
			{
				RunGameReadyChecks(newNumOfPlayersReady, 2);
			}

			// conditions when in debug mode. Debug mode allows the game to start with only 1 player (and maybe more).
			else {
				RunGameReadyChecks(newNumOfPlayersReady, 1);
			}

			numOfPlayersJoined = newNumOfPlayersReady;
			inputTimer += 1.0f * Time.deltaTime;
		}

		else {
			inputTimer = 0.0f;
		}
	}




	/**** ALL HELPER FUNCTIONS BELOW THIS POINT ARE NOW OUT OF DATE AND DO NOT WORK!!!!  ****/
	/* If any of the team needs need new helper functions, let David know on Discord. */
}
