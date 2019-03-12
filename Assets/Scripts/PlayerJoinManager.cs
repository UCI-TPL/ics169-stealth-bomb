using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;                   // this package and all methods/classes used by us are not our own. reference this!!!!


// NOTE: what do we do when more than 1 controller are connected at first, but the 2nd or a controller that wasnt the last one to connect disconnects?
public class PlayerJoinManager : MonoBehaviour {

	public bool usingNewPlayerJoinSystem = false;

	public bool playersLoseTheirNumberIfTheyLeave = true;

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
	[SerializeField]
	private Button backButton;
	private characterSelection selectionOP;

	//reference the mainMenuManager for setting PJActive to true;
	public GameObject mMManager;
	private MainMenuManager currentMenu; 

	public Image bButtonMask;
	public float fillSpeed = 0.2f;
	public float initialFillAmount = 0.18f; // originally 0.0f
	private float[] bButtonTimers;
	[HideInInspector]
	public bool bButtonActivated;

	public RectTransform[] joinPrompts;
	public RectTransform[] playersUI;
    public GameObject countdown;
    public Text countdownText;

	public string interactionNoise = "ButtonActivation";

	// [Tooltip("Reference to the game controller object.")]
	// public GameObject gameManager;
	// public string gameManagerName = "GameController";
	// private ActivePlayerManager playerManager;

	bool[] playersJoined;
	bool[] playersReady;
	bool[] controllersConnected;
	// an array of length 4, each space stores the controller number that the player will use in the rest of the game. This is passed to the game manager.
	// [HideInInspector]
	// [SerializeField]
	private int[] inputControllerNumbers;
	// the inverse of inputControllerNumbers. each index represents a controller and stores int that represents which player is using that controller
	// [HideInInspector]
	// [SerializeField]
	private int[] controllersToPlayers;

	[HideInInspector]
	// [SerializeField]
	public int numOfTimesMenuLoaded = 0;

	int notAssignedController = -1;
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
	private float countdownTimer;

	private InputManager input;

	private bool[] playersControlsGuideActive;   // temp private variable to represent which player has the controller guide showing while the real UI implementation isnt ready.

    private float player1X = -300;
    private float player2X = -100;
    private float player3X = 100;
    private float player4X = 300;

	private float fourPlayersWidth = 1000;
	private float fourPlayersHeight = 1500;

	private int[] defaultInputControllerNumbers;

	private bool alreadyLoadingScene;


    public Color[] Colors = new Color[6]; //this is basically hardcoding in the colors but it is quick

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
		// if (!usingNewPlayerJoinSystem)
			selectionOP = playersObject.GetComponent<characterSelection>();
		
		currentMenu = mMManager.GetComponent<MainMenuManager>();
	}

	// Use this for initialization
	private void Start () {
		input = InputManager.inputManager;
		players = new PlayerIndex[4];
		currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		inputControllerNumbers = new int[4];
		controllersToPlayers = new int[4];

		// assigning base values to controller arrays
		for (int i = 0; i < inputControllerNumbers.Length; i++) {
			inputControllerNumbers[i] = notAssignedController;
			controllersToPlayers[i] = notAssignedController;
		}

		playerTimers = new float[4];
		playersJoined = new bool[4];
		playersReady = new bool[4];
		controllersConnected = new bool[4];
		Player[] playerObjects = GameManager.instance.players;
		// if (GameManager.instance.players == null) {
		// 	Debug.Log("GameManager players array is null.");
		// }
		// else {
		// 	Debug.Log("GameManager players: " + Game)
		// }
		if (GameManager.instance.players != null) {
			playerObjects = GameManager.instance.players;
			// string playerInfo = "Players: ";
			// for (int i = 0; i < playerObjects.Length; i++) {
			// 	if (playerObjects[i] != null)
			// 		playerInfo += "player " + playerObjects[i].playerNumber + " controller number = " + playerObjects[i].inputControllerNumber + " ,";
			// }
			// Debug.Log(playerInfo);
		}
		else {
			// Debug.Log("GameManager players array is null.");
		}
		// Debug.Log("inputControllerNumbers length = " + inputControllerNumbers.Length + ", controllersToPlayers length = " + controllersToPlayers.Length);
		// if (inputControllerNumbers == null || controllersToPlayers == null || inputControllerNumbers.Length == 0 || controllersToPlayers.Length == 0) {
		// 	inputControllerNumbers = new int[4];
		// 	controllersToPlayers = new int[4];
		// 	numOfTimesMenuLoaded = 0;
		// }
		defaultInputControllerNumbers = new int[] { 0, 1, 2, 3 };
		playersControlsGuideActive = new bool[4];
		bButtonTimers = new float[4];
		bButtonActivated = false;
		countdown.SetActive(false);
		countdownTimer = 3.0f;
		alreadyLoadingScene = false;
		numOfTimesMenuLoaded += 1;

		for (int i = 0; i < playersJoined.Length; i++) {
			// Debug.Log("playersReady index=" + i);
			// InputManager.inputManager.controllers[i].confirm.OnDown.AddListener( () => ReadyPlayer(i) );
			// input.controllers[i].confirm.OnDown.AddListener( delegate { ReadyPlayer(0); } );
			players[i] = (PlayerIndex) i;
			playersJoined[i] = false;
			playersReady[i] = false;
			controllersConnected[i] = false;
			playersControlsGuideActive[i] = false;
			bButtonTimers[i] = initialFillAmount;
			playerTimers[i] = 0.0f;
			AssignControllerEvents(i);
			if (usingNewPlayerJoinSystem) {
				// first time in menu
				if (GameManager.instance.players == null) {
					inputControllerNumbers[i] = notAssignedController;   // represents they have not been assigned a controller yet
					controllersToPlayers[i] = notAssignedController;
				}
				// returning to menu from gameplay
				else {
					if (playerObjects[i] != null) {
						inputControllerNumbers[i] = playerObjects[i].inputControllerNumber;
						controllersToPlayers[inputControllerNumbers[i]] = i;
					}
				}
				ResetPlayer(i);
			}
			else {
				controllersToPlayers[i] = i;
				inputControllerNumbers[i] = i;
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
		if (!alreadyLoadingScene) {
			if (!usingNewPlayerJoinSystem) {
				UpdateOldVersion();
			}
			else {
				UpdateNewVersion();
			}

			float farthestTimer = 0.0f;
			if (PlayerJoinScreenActive) {
				for (int i = 0; i < bButtonTimers.Length; i++) {
					if (currentStates[i].IsConnected && input.controllers[i].cancel.Pressed) {
						bButtonTimers[i] += fillSpeed * Time.deltaTime;
					}
					else {
						bButtonTimers[i] = initialFillAmount;
					}

					if (farthestTimer < bButtonTimers[i])
						farthestTimer = bButtonTimers[i];
				}

				bButtonMask.fillAmount = farthestTimer;
				if (farthestTimer >= 1.0f) {
					bButtonActivated = true;
					// may want to remove interaction noise for exiting later!!
					PlayInteractionNoise();
					backButton.onClick.Invoke();
					// farthestTimer = initialFillAmount;
				}
			}
		}
	}


	///// Helper Methods /////


	// Callback helper methods for player input //

	// helper callback method that readies the player using the controller specified by the parameter/index.
	//Problem!!!!! Rejoining kicks controller to next player instead of getting the same one back
	private void PlayerJoin(int controllerIdx) {
		// Debug.Log("ReadyPlayer called for player " + playerIdx + ".");
		// Debug.Log("input controller list before: " + inputControllerNumbers[0] + "," + inputControllerNumbers[1] + "," + inputControllerNumbers[2] + "," + inputControllerNumbers[3]);

		// Player has been assigned a controller or has not joined yet
		if (CanPlayerPressButton(controllerIdx) && (controllersToPlayers[controllerIdx] == notAssignedController || playersJoined[inputControllerNumbers[controllerIdx]] == false) /* && playersJoined[controllersToPlayers[controllerIdx]] == false */) {
			// Display the UI element showing the player has confirmed he/she is ready to play.
			// Debug.Log("Player " + players[i] + " is ready to play!");

			if (!usingNewPlayerJoinSystem) {
				playersJoined[controllerIdx] = true;
				//calling UI -Kyle
				selectionOP.playerIs(controllerIdx);
				selectionOP.playerIsReady();
			}
			else {
				int i = 0;
				// if players do not have a controller.
				if (controllersToPlayers[controllerIdx] == notAssignedController) {
					for (; i < inputControllerNumbers.Length; i++) {
						if (inputControllerNumbers[i] == notAssignedController) {
							inputControllerNumbers[i] = controllerIdx;
							controllersToPlayers[controllerIdx] = i;
							break;
						}
					}
				}
				// the case where players have already been assigned one.
				else {
					i = controllersToPlayers[controllerIdx];
				}

				if (i < 4) {
					Debug.Log("Player " + (i + 1) + " joined.");
					playersJoined[i] = true;
					joinPrompts[i].gameObject.SetActive(false);

                    int colorIndex = GameManager.instance.AssignPlayerColor(i); //use playerNumber to receive a color

                    

                    playersUI[i].gameObject.SetActive(true);
                    SetUIColor(i, colorIndex);
                    //calling UI -Kyle
                    selectionOP.playerIs(i);
					selectionOP.playerConnected();
				}
			}
		}
		else if (CanPlayerPressButton(controllerIdx) && controllersToPlayers[controllerIdx] != notAssignedController) {
			if (usingNewPlayerJoinSystem) {
				PlayInteractionNoise();
				ReadyPlayer(controllerIdx);
			}
		}
		// Debug.Log("input controller list after: " + inputControllerNumbers[0] + "," + inputControllerNumbers[1] + "," + inputControllerNumbers[2] + "," + inputControllerNumbers[3]);
	}

	// helper callback method that unreadies the player using the controller specified by the parameter/index, or returns game to main menu panel.
	private void SetPlayerBack(int controllerIdx) {
		if (CanPlayerPressButton(controllerIdx)) {
			if (controllersToPlayers[controllerIdx] != notAssignedController && playersJoined[controllersToPlayers[controllerIdx]] == true) {
				if (usingNewPlayerJoinSystem) {
					// if player has already pressed start (readied up), then it will take the player back to previous stage
					if (playersReady[controllersToPlayers[controllerIdx]] == true) {
						playersReady[controllersToPlayers[controllerIdx]] = false;
						playersUI[controllersToPlayers[controllerIdx]].gameObject.SetActive(true);
						selectionOP.playerIs(controllersToPlayers[controllerIdx]);
						selectionOP.playerConnected();
					}
					// otherwise, player will be unjoin the lobby and will have to press A again to rejoin.
					else {
						// TEST THIS!!!!!
						ToggleControllerGuide(controllersToPlayers[controllerIdx], false);
						playersJoined[controllersToPlayers[controllerIdx]] = false;
						playersUI[controllersToPlayers[controllerIdx]].gameObject.SetActive(false);
						joinPrompts[controllersToPlayers[controllerIdx]].gameObject.SetActive(false);
						if (playersLoseTheirNumberIfTheyLeave) {
							inputControllerNumbers[controllersToPlayers[controllerIdx]] = notAssignedController;
							controllersToPlayers[controllerIdx] = notAssignedController;
						}
					}
				}
				else {
					playersJoined[controllerIdx] = false;
					// Debug.Log("Player " + players[i] + " is NOT ready to play!");

					//calling UI -Kyle
					selectionOP.playerIs(controllerIdx);
					//selectionOP.playerIsNotReady();
					selectionOP.playerConnected();
				}

				// joinPrompts[playerIdx].gameObject.SetActive(true);
			}

			// else {
			// 	GoBackToMainMenu();
			// }
		}
	}

	// helper callback method that toggles on or off the specified player's personal controller guide on their player box. (will later only work with new implementation)
	// NOTE: controllerIdx represents the controller the player is using, not the player itself
	private void ShowPlayerControls(int controllerIdx) {
		// player needs to join in order to see their controls
		if (CanPlayerPressButton(controllerIdx) && controllersToPlayers[controllerIdx] != notAssignedController 
												&& playersJoined[controllersToPlayers[controllerIdx]] == true) {
			if (!playersControlsGuideActive[controllersToPlayers[controllerIdx]]) {
				Debug.Log("Player " + (controllersToPlayers[controllerIdx] + 1) + " turned on their controller guide.");
				// the controller guide should always be the last child in the index
				ToggleControllerGuide(controllersToPlayers[controllerIdx], true);
			}
			else {
				Debug.Log("Player " + (controllersToPlayers[controllerIdx] + 1) + " turned off their controller guide.");
				ToggleControllerGuide(controllersToPlayers[controllerIdx], false);
			}

			// temp toggle on or off for controller guide
			// playersControlsGuideActive[controllersToPlayers[controllerIdx]] = !playersControlsGuideActive[controllersToPlayers[controllerIdx]];
		}
	}

    public void SetUIColor(int i, int colorIndex) // i is player number
    {
        playersUI[i].gameObject.GetComponent<Image>().color = Colors[colorIndex]; //changing the color of the player panel
        playersUI[i].Find("Border").GetComponent<Image>().color = Colors[colorIndex];
        playersUI[i].Find("ChangeColorBackground").Find("Background").GetComponent<Image>().color = Colors[colorIndex];
        playersUI[i].Find("newish Xbox controller guide").GetComponent<Image>().color = Colors[colorIndex] * new Color(1, 1, 1, 0.5f);
    }

    public void SwitchColors(int controllerIdx) // controller number as parameter
    {
        Debug.Log("controllersToPlayers: " + controllersToPlayers.Length.ToString() + " : " + controllerIdx.ToString());
        Debug.Log("playersJoined: " + playersJoined.Length.ToString() + " : " + controllersToPlayers[controllerIdx].ToString());
		if (CanPlayerPressButton(controllerIdx) && playersJoined[controllersToPlayers[controllerIdx]] == true) {
			int colorIndex = GameManager.instance.ExchangeColors(controllersToPlayers[controllerIdx]);
			if (colorIndex == -1) //this means that the player was not assigned a color yet!
				return;
			SetUIColor(controllersToPlayers[controllerIdx], colorIndex);
		}
    }

	// helper callback method that starts the game if the player and others are ready (old update() version).
	private void StartGame(int playerIdx) {
		if (CanPlayerPressButton(playerIdx) && playersJoined[playerIdx] == true) {
			if ((!debugMode && numOfPlayersJoined >= 2) || (debugMode && numOfPlayersJoined >= 1))
				GameManager.instance.StartGame(GetPLayerReadyStatusList(), defaultInputControllerNumbers);
		}
	}

	private void ReadyPlayer(int controllerIdx) {
		if (CanPlayerPressButton(controllerIdx) && controllersToPlayers[controllerIdx] != notAssignedController 
												&& playersJoined[controllersToPlayers[controllerIdx]] == true) {
			playersReady[controllersToPlayers[controllerIdx]] = true;
			selectionOP.playerIs(controllersToPlayers[controllerIdx]);
			selectionOP.playerIsReady();
		}
	}


	// General Helper Methods //

	// helper method that returns whether or not the specified player can enter input.
	private bool CanPlayerPressButton(int playerIdx) {
		return currentStates[playerIdx].IsConnected && PlayerJoinScreenActive && inputTimer >= cooldown;
	}


	// helper method that toggles player's (specified by playerIdx) controller guide on or off.
	private void ToggleControllerGuide(int playerIdx, bool turnOn) {
		playersUI[playerIdx].GetChild(1).gameObject.SetActive(turnOn);
		playersControlsGuideActive[playerIdx] = turnOn;
	}


	// helper function exits out of pre-game lobby to main menu
	public void ExitPreGameLobby() {
		Debug.Log("back button invoked");
		bButtonActivated = true;
		for (int idx = 0; idx < playersJoined.Length; idx++) {
			ResetPlayer(idx);
		}
		int[] menuSettings = new int[2];
		menuSettings[0] = 1;
		menuSettings[1] = 1;
		currentMenu.setMenu(menuSettings);
	}


	// helper method that assigns what functions should be called by what input/event.
	// NOTE: when adding a new listener, the function you add needs to 
	//       take in an int that represents the CONTROLLER NUMBER, NOT THE PLAYER NUMBER!! player number might not be assigned 
	//       and is always in flux because controller number is not always same as player number.
	private void AssignControllerEvents(int controllerIdx) {
		// A button, B button, and Start button has been assigned. Still need to assign Y button.
		switch (controllerIdx) {
			case 0:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PlayerJoin(0) );
				input.controllers[controllerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(0) );
				input.controllers[controllerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(0) );
                input.controllers[controllerIdx].xbutton.OnDown.AddListener(() => SwitchColors(0));
                if (!usingNewPlayerJoinSystem)
					input.controllers[controllerIdx].start.OnDown.AddListener( () => StartGame(0) );
				// else 
					// input.controllers[controllerIdx].start.OnDown.AddListener( () => ReadyPlayer(0) );
				break;
			case 1:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PlayerJoin(1) );
				input.controllers[controllerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(1) );
				input.controllers[controllerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(1) );
                input.controllers[controllerIdx].xbutton.OnDown.AddListener(() => SwitchColors(1));
                if (!usingNewPlayerJoinSystem)
					input.controllers[controllerIdx].start.OnDown.AddListener( () => StartGame(1) );
				// else 
					// input.controllers[controllerIdx].start.OnDown.AddListener( () => ReadyPlayer(1) );
				break;
			case 2:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PlayerJoin(2) );
				input.controllers[controllerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(2) );
				input.controllers[controllerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(2) );
                input.controllers[controllerIdx].xbutton.OnDown.AddListener(() => SwitchColors(2));
                if (!usingNewPlayerJoinSystem)
					input.controllers[controllerIdx].start.OnDown.AddListener( () => StartGame(2) );
				// else 
					// input.controllers[controllerIdx].start.OnDown.AddListener( () => ReadyPlayer(2) );
				break;
			case 3:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PlayerJoin(3) );
				input.controllers[controllerIdx].cancel.OnDown.AddListener( () => SetPlayerBack(3) );
				input.controllers[controllerIdx].Switch.OnDown.AddListener( () => ShowPlayerControls(3) );
                input.controllers[controllerIdx].xbutton.OnDown.AddListener(() => SwitchColors(3));
                if (!usingNewPlayerJoinSystem)
					input.controllers[controllerIdx].start.OnDown.AddListener( () => StartGame(3) );
				// else 
					// input.controllers[controllerIdx].start.OnDown.AddListener( () => ReadyPlayer(3) );
				break;
			default:
				Debug.Log("variable playerIdx out of range!");
				break;
		}
	}


	private void TestConfirm() {
		Debug.Log("button A pressed.");
	}

	private void PlayInteractionNoise() {
		if (interactionNoise != null && interactionNoise != "")
            GameManager.instance.audioManager.Play(interactionNoise);
	}


	// Helper function that checks if players can start the game and, if so, adds UI to indicate this state.
	// Also, still manages taking in MMK input since input manager's MMK support has not been integrated into the join manager yet
	void RunGameReadyChecks(int newNumOfPlayersReady, int minNumOfPlayers) {
		// Checks if enough players have confirmed they are ready.
		if (newNumOfPlayersReady >= minNumOfPlayers) {
			// old logic
			// Display the UI element showing that the game is ready to start.
			// Debug.Log("Game is Ready to start!");
			if (!usingNewPlayerJoinSystem) {
				//calling UI -Kyle
				selectionOP.gameIsReady();
			}
			// new logic
			else {
				countdown.SetActive(true);
				countdownText.text = "Starting in <color=#f7ff63>" + ((int)countdownTimer + 1).ToString() + "</color>";
                countdownText.rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.1f, 1.1f, 1), countdownTimer - Mathf.Floor(countdownTimer));
				countdownTimer -= 1.0f * Time.deltaTime;

				if (countdownTimer <= 0.0f) {
					if (!debugMode)
						GameManager.instance.StartGame(GetPLayerReadyStatusList(), inputControllerNumbers);
					else 
						GameManager.instance.StartGame(GetPLayerReadyStatusList(), defaultInputControllerNumbers); // assumes there is only 1 plugged-in controller
					
					alreadyLoadingScene = true;
				}
			}
		}
		else if (newNumOfPlayersReady < minNumOfPlayers /*&& numOfPlayersReady >= 2*/) {
			// Turn off the UI element showing that the game is ready to start.
			// Debug.Log("There are not enough players for the game to start!");
			if (!usingNewPlayerJoinSystem) {
				//calling UI -Kyle
				selectionOP.gameIsNotReady();
			}
			else {
				countdownTimer = 3.0f;
				countdown.SetActive(false);
			}
		}

		// if conditions met, start the match (for old version with pc controls)
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
			// NOTE: controllersToPlayers[i] gives the player index that controller is linked to
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState(players[i]);
				if (currentStates[i].IsConnected || i != playerUsingKeyboardIdx || !playerUsingMouseAndKeyboard) {
					if (currentStates[i].IsConnected) {
						if (!prevStates[i].IsConnected) {
							controllersConnected[i] = true;
							if (controllersToPlayers[i] != notAssignedController)
								playersJoined[controllersToPlayers[i]] = false;
							// playersJoined[i] = false;
							
						}
					}
					else if (!currentStates[i].IsConnected /* && prevStates[i].IsConnected*/) {
						controllersConnected[i] = false;
						if (controllersToPlayers[i] != notAssignedController) {
							playersJoined[controllersToPlayers[i]] = false;
							ResetPlayer(controllersToPlayers[i]);
						}
					}
				}
				// condition for if player is using mouse and keyboard
				else {
					// selectionOP.playerIs(i);
					// selectionOP.playerIsReady();
					// PlayerJoin()
					// playersJoined[i] = true;
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
				if (i < numOfControllersConnected /*&& !playersJoined[i]*/) {
					if (!playersJoined[i]) {
						joinPrompts[i].gameObject.SetActive(true);
						playersUI[i].gameObject.SetActive(false);

					}
					else {
						joinPrompts[i].gameObject.SetActive(false);
					}
					//Vector3 newPosition;
					//if (i == 0) {
					//	if (numOfControllersConnected < 3) 
					//		newPosition = new Vector3(-100.0f, 0.0f, 0.0f);
					//	else
					//		newPosition = new Vector3(-300.0f, 0.0f, 0.0f);
					//}
					//else {
					//	newPosition = new Vector3(joinPrompts[i-1].transform.localPosition.x + 200.0f, 0.0f, 0.0f);
					//	// joinPrompts[i].transform.localPosition = newPosition;
					//}
					//joinPrompts[i].transform.localPosition = newPosition;
					//playersUI[i].transform.localPosition = newPosition;
				}
				else {
					joinPrompts[i].gameObject.SetActive(false);
				}
			}

			// This loop checks to see which players have A to confirm they are ready. <- description not correct anymore
			for (int i = 0; i < 4; i++) {
				// Debug.Log("Player " + (i + 1) + " Timer = " + playerTimers[i]);
				if (currentStates[i].IsConnected && playerTimers[i] >= cooldown) {
					// sets players back to main menu and resets their ready status.
					if (currentStates[i].Buttons.B == ButtonState.Pressed && controllersToPlayers[i] != notAssignedController && playersJoined[controllersToPlayers[i]] == false) {
						
					}
					else if (controllersToPlayers[i] != notAssignedController && playersJoined[controllersToPlayers[i]] == false) {
						// playersJoined[i] = false;
						// Debug.Log("Player " + players[i] + " is NOT ready to play!");

						playersReady[controllersToPlayers[i]] = false;
						// //calling UI -Kyle
						selectionOP.playerIs(controllersToPlayers[i]);
						selectionOP.playerConnected();

						playersUI[controllersToPlayers[i]].gameObject.SetActive(false);
						joinPrompts[controllersToPlayers[i]].gameObject.SetActive(true);
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
			for (int i = 0; i < playersReady.Length; i++) {
				if (playersReady[i] == true)
					newNumOfPlayersReady++;
			}

			if (usingNewPlayerJoinSystem) {
				int newNumOfPlayersJoined = 0;
				for (int i = 0; i < playersJoined.Length; i++) {
					if (playersJoined[i] == true) 
						newNumOfPlayersJoined++;
				}
				numOfPlayersJoined = newNumOfPlayersJoined;
			}

			// the normal conditions needed for the game to start
			if (!debugMode) 
			{
				// case for when you have more than 2 controllers plugged in and need more than 2 players to have joined
				if (numOfControllersConnected > 2 && numOfPlayersJoined > 2)
					RunGameReadyChecks(newNumOfPlayersReady, numOfPlayersJoined /* numOfControllersConnected */);
				else // need to have at least 2 players plugged in and ready to start game
					RunGameReadyChecks(newNumOfPlayersReady, 2);
			}

			// conditions when in debug mode. Debug mode allows the game to start with only 1 player (and maybe more).
			else {
				RunGameReadyChecks(newNumOfPlayersReady, 1);
			}

			if (!usingNewPlayerJoinSystem)
				numOfPlayersJoined = newNumOfPlayersReady;
			inputTimer += 1.0f * Time.deltaTime;
		}

		else {
			bButtonActivated = false;
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
			if (playersLoseTheirNumberIfTheyLeave) {  // switchable option in inspector
				if (inputControllerNumbers[playerIdx] != notAssignedController && GameManager.instance.players == null) {
					controllersToPlayers[inputControllerNumbers[playerIdx]] = notAssignedController;
					inputControllerNumbers[playerIdx] = notAssignedController;
				}
			}
			ToggleControllerGuide(playerIdx, false);
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
