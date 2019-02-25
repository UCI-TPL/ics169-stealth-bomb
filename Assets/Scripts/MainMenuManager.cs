using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XInputDotNetPure;

public class MainMenuManager : MonoBehaviour {

	/*
	int menu represents the current menu
	0 - quit								1 - main menu
	2 - character selection menu			3 - setting menu
	4 - setting : player button mapping	
	 */
	private static int menu = 0;
	
	//GameObject reference to menu panels
	public GameObject mainMenuPanel;
	public GameObject selectionMenuPanel;
	public GameObject remappingMenuPanel;
    public GameObject AudioPanel;

	public GameObject CreditsPanel;

	public AudioMenuManager audioMenuManager;
	public GameObject customMappingPanel;

	// public GameObject xboxControllerDisplay;

	// public GameObject pcControllerDisplay;

	public bool newVersion = true;

	private GameObject btn;
	private ButtonController b;

	public string nameOfButtonSoundEffect = "Bow";


	PlayerIndex player1 = PlayerIndex.One;
	bool playerSet;
	GamePadState currentState;
	GamePadState prevState;
	GamePadState[] currentStates;
	GamePadState[] prevStates;
	PlayerIndex[] players;
	private int currentMainMenuButton;
	private int prevMainMenuButton;
	private int currentSelectionMenuButton;
	private int currentRemappingMenuButton;
	private int currentCustomMappingButton;

	// xbox controller variables
	public float controllerStickDeadZone = 0.5f; // keep this between 0 and 1.
	public float cooldown = 0.25f;
	private float timer;	
	public float buttonCoolDown = 0.5f;
	private float buttonTimer;
	private bool hasMoved;
	private bool hasClicked;

	private InputManager input;

	public int getCurrentPanel()
	{ return menu; }

	private void Start()
	{
		// currentMainMenuButton = 1;
		int[] menuSettings = new int[2];
		menuSettings[0] = 1;
		menuSettings[1] = 1;
		setMenu(menuSettings);
		input = InputManager.inputManager;
		playerSet = false;
		CheckForAndSkipDeactivatedButtons(false);
		// mainMenuButtons(currentMainMenuButton);
		prevMainMenuButton = currentMainMenuButton;
		currentSelectionMenuButton = 2;
		currentRemappingMenuButton = 1;
		timer = cooldown;   // 7.5f
		buttonTimer = 0.0f;
		hasMoved = false;
		InputManager.inputManager.UseMouseAndKeyboardForFirstDisconnectedPlayer(false);
		currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		players = new PlayerIndex[4];

		for (int i = 0; i < 4; i++) {
			players[i] = (PlayerIndex) i;
			AssignControllerEvents(i);
			// InputManager.inputManager.controllers[i].confirm.OnDown.AddListener();
			// InputManager.inputManager.controllers[i].cancel.OnDown.AddListener();
		}
	}

	void Update() {
		// Debug.Log("cooldown = " + cooldown);
		prevMainMenuButton = currentMainMenuButton;

		if (newVersion)
			ControllerNavigationNewVersion();
		else
			ControllerNavigationOldVersion();

		if (getCurrentPanel() == 1) {
			buttonTimer += 1.0f * Time.deltaTime;
		}
		else {
			buttonTimer = 0.0f;
		}

		// if (prevMainMenuButton != currentMainMenuButton)
		// 		Debug.Log("current main menu button number: " + currentMainMenuButton);
		
		// GameManager.instance.inputManager.controllers[0].
	}

	// rough version of being able to navigate main menu with more than 1 controller
	// NOTE: might be modified/replaced later to make it more clean (if menu controls are integrated with input manager)
	private void ControllerNavigationNewVersion() {
		// loops through all player indexes (input)
		for (int i = 0; i < players.Length; i++) {
			prevStates[i] = currentStates[i];
			currentStates[i] = GamePad.GetState(players[i]);
			// Debug.Log("Test 1");

			if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
				// Debug.Log("Test 1.5: timer = " + timer);
			// assuming cooldown is finished, allow input from controller
				if (timer >= cooldown) {
					// Debug.Log("Test 2");
					hasMoved = false;
					// makes sure controller is connected and there is no other input since cooldown finished
					if (currentStates[i].IsConnected && !hasMoved) {
						if (mainMenuPanel.activeSelf == true && getCurrentPanel() == 1) {
							// Debug.Log("Test 3");
							// Debug.Log("Left Thumbstick input = " + currentStates[i].ThumbSticks.Left.Y);
							if (currentStates[i].ThumbSticks.Left.Y > controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
								currentMainMenuButton--;
								CheckAndMoveCursorToBottom();
								CheckForAndSkipDeactivatedButtons(true);
								// mainMenuButtons(currentMainMenuButton);
								// Debug.Log("cursor moved up to " + currentMainMenuButton);
								hasMoved = true;
							}
							else if (currentStates[i].ThumbSticks.Left.Y < -controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
								currentMainMenuButton++;
								CheckAndMoveCursorToTop();
								CheckForAndSkipDeactivatedButtons(false);
								// mainMenuButtons(currentMainMenuButton);
								// Debug.Log("cursor moved down to " + currentMainMenuButton);
								hasMoved = true;
							}

							mainMenuButtons(currentMainMenuButton);
						}

						// if (selectionMenuPanel.activeSelf == true) {
						// 	// if (currentStates[i].ThumbSticks.Left.X > 0) {
						// 	// 	currentSelectionMenuButton++;
						// 	// 	if (currentSelectionMenuButton > 3) {
						// 	// 		currentSelectionMenuButton = 2;
						// 	// 	}
						// 	// 	selectionMenuButtons(currentSelectionMenuButton);
						// 	// 	hasMoved = true;
						// 	// }
						// }

						// if (remappingMenuPanel.activeSelf == true)
						// {

						// 	// update button pos
						// 	if (currentStates[i].ThumbSticks.Left.Y < -controllerStickDeadZone)
						// 	{
						// 		currentRemappingMenuButton++;
						// 		hasMoved = true;
						// 	}
						// 	else if (currentStates[i].ThumbSticks.Left.Y > controllerStickDeadZone)
						// 	{
						// 		currentRemappingMenuButton--;
						// 		hasMoved = true;
						// 	}
							
						// 	// fix outbound pos
						// 	if (currentRemappingMenuButton > 5)
						// 	{
						// 		currentRemappingMenuButton = 1;
						// 	}
						// 	else if (currentRemappingMenuButton < 1)
						// 	{
						// 		currentRemappingMenuButton = 5;
						// 	}
							
						// 	remappingMenuButtons(currentRemappingMenuButton);
							
						// }

						// if (AudioPanel.activeSelf == true)

						if (hasMoved) 
						{
							timer = 0.0f;
							timer += 1.0f * Time.deltaTime;
						}
					}
				}

				// NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!
				// getCurrentPanel() (or menu) needs to be <= 1, indicating that we are currently in the main menu, or we pressed quit while in the Unity editor.
				// if (currentStates[i].Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() == 1 && btn.transform.GetSiblingIndex() == currentMainMenuButton) {
				// 	// Debug.Log("clicking");
				// 	// b.onClick.Invoke();
				// 	ActivateButton();
				// }
				// Debug.Log("current main menu button: " + currentMainMenuButton);
			}
		}

		if (hasMoved && timer < cooldown) 
			timer += 1.0f * Time.deltaTime;
	}

	// void LateUpdate() {
	// 	if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
	// 		mainMenuButtons(currentMainMenuButton);
	// 	}
	// }

	// Callback method for pressing/clicking on a unity button (planned to be A button)
	private void ActivateSelectedMenuOption(int controllerIdx) {
		if (CanPlayerPressButton(controllerIdx)) {
			// Debug.Log("clicking/pressing:" + b.name);
			// Debug.Log("current main menu button upon pressing A = " + currentMainMenuButton);
			// b.onClick.Invoke();
			ActivateButton();
		}
	}


	// helper method that returns whether or not the specified player can enter input.
	private bool CanPlayerPressButton(int controllerIdx) {
		return currentStates[controllerIdx].IsConnected && (getCurrentPanel() == 1 || getCurrentPanel() == 6) && buttonTimer >= buttonCoolDown /* && btn.transform.GetSiblingIndex() == currentMainMenuButton*/;
	}

	private void ActivateButton() {
		// Debug.Log("clicking/pressing:" + b.name);
		// Debug.Log("current main menu button upon pressing A = " + currentMainMenuButton);
		// starts at 1 because 0 represents the game title.
		// for (int i = 1; i < mainMenuPanel.transform.childCount; i++) {
		// 	if (b.)
		// }
		// b.image.color = Color.red; //new Color(b.colors.pressedColor.r, b.colors.pressedColor.g, b.colors.pressedColor.b, b.colors.pressedColor.a);
		// b.onClick.Invoke();
		b.PressButton();
	}


	// helper method that assigns what functions should be called by what input/event.
	private void AssignControllerEvents(int controllerIdx) {
		// A button, B button, and Start button has been assigned. Still need to assign Y button.
		switch (controllerIdx) {
			case 0:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateSelectedMenuOption(0) );
				break;
			case 1:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateSelectedMenuOption(1) );
				break;
			case 2:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateSelectedMenuOption(2) );
				break;
			case 3:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateSelectedMenuOption(3) );
				break;
			default:
				Debug.Log("variable playerIdx out of range!");
				break;
		}
	}



	// old version of update: not really being used anymore
	private void ControllerNavigationOldVersion() {
		prevState = currentState;
		currentState = GamePad.GetState(player1);

		if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
		// for now, only player 1 should be able to control the main menu.
			if (currentState.IsConnected && timer >= cooldown) {
				if (mainMenuPanel.activeSelf == true) {
					if (currentState.ThumbSticks.Left.Y > controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
						currentMainMenuButton--;
						if (currentMainMenuButton < 1) {
							currentMainMenuButton = mainMenuPanel.transform.childCount - 1;
						}
						mainMenuButtons(currentMainMenuButton);
						hasMoved = true;
					}
					else if (currentState.ThumbSticks.Left.Y < -controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
						currentMainMenuButton++;
						if (currentMainMenuButton >= mainMenuPanel.transform.childCount) {
							currentMainMenuButton = 1;
						}
						mainMenuButtons(currentMainMenuButton);
						hasMoved = true;
					}
				}

				if (selectionMenuPanel.activeSelf == true) {
					if (currentState.ThumbSticks.Left.X > 0) {
						currentSelectionMenuButton++;
						if (currentSelectionMenuButton > 3) {
							currentSelectionMenuButton = 2;
						}
						selectionMenuButtons(currentSelectionMenuButton);
						hasMoved = true;
					}
				}
				if (hasMoved) 
				{
					timer = 0.0f;
					timer += 1.0f * Time.deltaTime;
				}
			}
			else {
				timer += 1.0f * Time.deltaTime;
			}

			// NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!!
			if (currentState.Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() != 2) {
				b.onClick.Invoke();
			}
			// Debug.Log("current main menu button: " + currentMainMenuButton);
		}
	}

	/*
	public methods for external usage
	 */
	//set menu to m and present to the corrsponding menu panel 
	// set selectedMainMenuButton to the index of the button in the menu panel m to highlight the button upon entering the menu panel (NOTE: only implemented with main menu panel)
	public void setMenu(/* int m, int selectedMainMenuButton,*/ int[] menuSettings)
	{ 
		// menu = m; 
		if (menuSettings[0] != 0) menu = menuSettings[0];
		EventSystem.current.SetSelectedGameObject(null);

		switch(menuSettings[0])
		{
			//Quit
			case 0:
				Debug.Log("Quit game.");
				Application.Quit();
				break;

			//Main menu
			case 1:
				// Debug.Log("to main menu");
				mainMenuPanel.SetActive(true);
				selectionMenuPanel.SetActive(false);
				remappingMenuPanel.SetActive(false);
				customMappingPanel.SetActive(false);
				CreditsPanel.SetActive(false);
                if(AudioPanel)
                    AudioPanel.SetActive(false);
				// if (selectedMainMenuButton != -1)
				// 	currentMainMenuButton = selectedMainMenuButton;
				// else {
				// 	currentMainMenuButton = 1;
				// }
				if (menuSettings.Length >= 2 && menuSettings[1] >= 0)
					currentMainMenuButton = menuSettings[1];
				else {
					currentMainMenuButton = 1;
				}
				// currentMainMenuButton = 1;
				mainMenuButtons(currentMainMenuButton);
				// currentMainMenuButton = 1;
				// Debug.Log("exiting main menu display");
				break;
			
			//Character Selection
			case 2:
				// Debug.Log("to selection");
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				remappingMenuPanel.SetActive(false);
				customMappingPanel.SetActive(false);
				// selectionMenuButtons(2);
				currentSelectionMenuButton = 2;
				InputManager.inputManager.UseMouseAndKeyboardForFirstDisconnectedPlayer(false);
				break;

			//remapBtn Menu (select which player)
			case 3:
				// Debug.Log("to remap menu");
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				remappingMenuPanel.SetActive(true);
				customMappingPanel.SetActive(false);
				// remappingMenuButtons(1);
				currentRemappingMenuButton = 1;
				// Debug.Log("exiting remapBtn menu display");
				break;

			//custom mapping Menu
			case 4: 
				// Debug.Log("to remapp");
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				remappingMenuPanel.SetActive(false);
				customMappingPanel.SetActive(true);
				// customMappingButtons(0);
				currentCustomMappingButton = 0;
				break;

			// audio settings Menu
			case 5:
				mainMenuPanel.SetActive(false);
				AudioPanel.SetActive(true);
				selectionMenuPanel.SetActive(false);
				remappingMenuPanel.SetActive(false);
				audioMenuManager.ResetAudioPanel();
				break;  // all setup is taken care of in another function;
			
			case 6:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				remappingMenuPanel.SetActive(false);
				customMappingPanel.SetActive(false);
				CreditsPanel.SetActive(true);
				// b = CreditsPanel.transform.GetComponentInChildren<Button>();
				// b.Select();
				break;
			
			default:
				break;		
		}
	}

	// public functions that can be used by UI inspector of onClick for buttons //

	public void ExitGame(GameObject exitButton) {
		int exitButtonIdx = exitButton.transform.GetSiblingIndex();
		Debug.Log("exit button sibling index: " + exitButtonIdx);
		if (exitButtonIdx == currentMainMenuButton)
			GoToMenu(0, 0);
	}

	public void OpenPreGameLobby() {
		GoToMenu(2, 0);
	}

	public void OpenRemappingScreen() {
		GoToMenu(3, 0);
	}

	public void OpenCreditsScreen() {
		GoToMenu(6, 0);
	}

	public void OpenAudioPanel() //things would be easier to read as seperate functions
    {
		GoToMenu(5, 0);
    }

	public void Mute() {
		Debug.Log("Mute button pressed.");
		GameManager.instance.Mute();
	}

	public void ExitRemappingScreen(GameObject settingsButton) {
		int settingsButtonIdx = settingsButton.transform.GetSiblingIndex();
		GoToMenu(1, settingsButtonIdx);
	}

	public void ExitAudio(GameObject audioButton) {
		int audioButtonIdx = audioButton.transform.GetSiblingIndex();
		GoToMenu(1, audioButtonIdx);
	}

	public void ExitCredits(GameObject creditsButton) {
		int creditsButtonIdx = creditsButton.transform.GetSiblingIndex();
		GoToMenu(1, creditsButtonIdx);
	}

	public void GoToMenu(int m, int selectedMainMenuButton) {
		// bool soundPlaying = false;
		// do { 
		// 	GameManager.instance.audioManager.IsSoundPlaying(nameOfButtonSoundEffect, out soundPlaying);
		// 	Debug.Log("is sound playing: " + soundPlaying);
		// } while (soundPlaying);
		
		// int[] menuSettings = new int[2];
		// menuSettings[0] = m;
		// menuSettings[1] = selectedMainMenuButton;
		// setMenu(menuSettings);

		StartCoroutine(OpenMenuDelayed(m, selectedMainMenuButton));
	}


	private IEnumerator OpenMenuDelayed(int m, int selectedMainMenuButton) {
		Sound s = GameManager.instance.audioManager.GetSound(nameOfButtonSoundEffect);
		if (s == null || b == null || b.gameObject.GetComponent<ButtonController>() == null) {
			yield return new WaitForSeconds(0.0f);
		}
		else {
			menu = m;
			yield return new WaitForSeconds(s.clip.length);
		}

		int[] menuSettings = new int[2];
		menuSettings[0] = m;
		menuSettings[1] = selectedMainMenuButton;
		Debug.Log("menu settings: " + menuSettings[0] + ", " + menuSettings[1]);
		setMenu(menuSettings);
	}


    public void SetVolume()
    {

    }

	// public void SelectControllerDisplay(int i) {
	// 	if (selectionMenuPanel.activeSelf == true) {
	// 		switch (i) {
	// 			case 0:
	// 				xboxControllerDisplay.SetActive(true);
	// 				pcControllerDisplay.SetActive(false);
	// 				break;
	// 			case 1:
	// 				xboxControllerDisplay.SetActive(false);
	// 				pcControllerDisplay.SetActive(true);
	// 				break;
	// 		}
	// 	}
	// }

	// public void ResetControllerDisplay(bool doesNothing) {
	// 	xboxControllerDisplay.SetActive(true);
	// 	pcControllerDisplay.SetActive(false);
	// }


	// helper method to check if the controller cursor/highlighter has gone too high in the buttons index and needs to be needs moved to the bottom of the menu.
	private void CheckAndMoveCursorToBottom() {
		if (currentMainMenuButton < 1) {
			currentMainMenuButton = mainMenuPanel.transform.childCount - 1;
		}
	}

	// helper method to check if the controller cursor/highlighter has gone too low in the buttons index and needs to be moved to the top of the menu.
	private void CheckAndMoveCursorToTop() {
		if (currentMainMenuButton >= mainMenuPanel.transform.childCount) {
			currentMainMenuButton = 1;
		}
	}

	// helper method to skip past any buttons that are disabled.
	private void CheckForAndSkipDeactivatedButtons(bool movedUp) {
		CheckForAndSkipDeactivatedButtonsRecursive(currentMainMenuButton, movedUp, 0);
	}

	// helper recursive method to skip past any buttons that are disabled.
	private void CheckForAndSkipDeactivatedButtonsRecursive(int idx, bool movedUp, int numOfButtonsSkipped) {
		if (idx != currentMainMenuButton) Debug.Log("parameter idx is not equal to currentMainMenuButton!");

		if (numOfButtonsSkipped < (mainMenuPanel.transform.childCount - 1) && !mainMenuPanel.transform.GetChild(idx).gameObject.activeSelf) {
			// player moved the thumbstick up to select a button visually above the current one in the main menu.
			if (movedUp) {
				--currentMainMenuButton;
				CheckAndMoveCursorToBottom();
			}
			// player moved the thumbstick down to select a button visually below the current one in the main menu.
			else {
				++currentMainMenuButton;
				CheckAndMoveCursorToTop();
			}

			CheckForAndSkipDeactivatedButtonsRecursive(currentMainMenuButton, movedUp, numOfButtonsSkipped + 1);
		}

		// safe condition to stop potential infinite loop.
		if (numOfButtonsSkipped >= (mainMenuPanel.transform.childCount - 1))
			Debug.Log("All main menu buttons are disabled!");
	}


	/*
	functions below will be called when the corsponding panel is active
	to enable the button highlight for controller
	 */
	private void mainMenuButtons( int i )
	{
		btn = mainMenuPanel.transform.GetChild(i).gameObject;
		// if (prevMainMenuButton != currentMainMenuButton)
		// 	Debug.Log("main menu button selected: " + btn.name);
		_buttonSelect();
	}

	private void selectionMenuButtons( int i )
	{
		btn = selectionMenuPanel.transform.GetChild(i).gameObject;
		_buttonSelect();
	}

	private void remappingMenuButtons( int i )
	{
		btn = remappingMenuPanel.transform.GetChild(i).gameObject;
		_buttonSelect();
	}

	private void customMappingButtons( int i )
	{
		btn = customMappingPanel.transform.GetChild(i).gameObject;
		_buttonSelect();
	}

	private void _buttonSelect()
	{
		Debug.Log(btn.name + " button selected");
		if (b == null || b.gameObject != btn)
			b = btn.GetComponent<ButtonController>();
		b.Select();
	}

	// // old, unclean version of Update, still works, but keeping it just in case
	// void Update() {
	// 	// code modified from 3rd party script: XInputTestCS.cs
	// 	// if (!playerSet || !prevState.IsConnected) {
	// 	// 	for (int i = 0; i < 4; i++) {
	// 	// 		PlayerIndex testPlayer = (PlayerIndex) i;
	// 	// 		//player = (PlayerIndex) i;
	// 	// 		GamePadState testState = GamePad.GetState(testPlayer);
	// 	// 		if (testState.IsConnected) {
	// 	// 			playerSet = true;
	// 	// 			player1 = testPlayer;
	// 	// 		}
	// 	// 	}
	// 	// }
	// 	// prevState = currentState;
	// 	// currentState = GamePad.GetState(player1);
	// 	// end of borrowed/modified code

	// 	// Debug.Log("cooldown = " + cooldown);

	// 	if (newVersion)
	// 		ControllerNavigationNewVersion();
	// 	else
	// 		ControllerNavigationOldVersion();
		
	// 	// prevState = currentState;
	// 	// currentState = GamePad.GetState(player1);

	// 	// if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
	// 	// // for now, only player 1 should be able to control the main menu.
	// 	// 	if (currentState.IsConnected && timer >= cooldown) {
	// 	// 		if (mainMenuPanel.activeSelf == true) {
	// 	// 			if (currentState.ThumbSticks.Left.Y > 0.0f /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
	// 	// 				currentMainMenuButton--;
	// 	// 				if (currentMainMenuButton < 1) {
	// 	// 					currentMainMenuButton = mainMenuPanel.transform.childCount - 1;
	// 	// 				}
	// 	// 				mainMenuButtons(currentMainMenuButton);
	// 	// 				hasMoved = true;
	// 	// 			}
	// 	// 			else if (currentState.ThumbSticks.Left.Y < 0.0f /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
	// 	// 				currentMainMenuButton++;
	// 	// 				if (currentMainMenuButton >= mainMenuPanel.transform.childCount) {
	// 	// 					currentMainMenuButton = 1;
	// 	// 				}
	// 	// 				mainMenuButtons(currentMainMenuButton);
	// 	// 				hasMoved = true;
	// 	// 			}
	// 	// 		}

	// 	// 		if (selectionMenuPanel.activeSelf == true) {
	// 	// 			if (currentState.ThumbSticks.Left.X > 0) {
	// 	// 				currentSelectionMenuButton++;
	// 	// 				if (currentSelectionMenuButton > 3) {
	// 	// 					currentSelectionMenuButton = 2;
	// 	// 				}
	// 	// 				selectionMenuButtons(currentSelectionMenuButton);
	// 	// 				hasMoved = true;
	// 	// 			}
	// 	// 		}
	// 	// 		if (hasMoved) 
	// 	// 		{
	// 	// 			timer = 0.0f;
	// 	// 			timer += 1.0f * Time.deltaTime;
	// 	// 		}
	// 	// 	}
	// 	// 	else {
	// 	// 		timer += 1.0f * Time.deltaTime;
	// 	// 	}

	// 	// 	// NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!!
	// 	// 	if (currentState.Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() != 2) {
	// 	// 		b.onClick.Invoke();
	// 	// 	}
	// 	// 	// Debug.Log("current main menu button: " + currentMainMenuButton);
	// 	// }
	// }

}
