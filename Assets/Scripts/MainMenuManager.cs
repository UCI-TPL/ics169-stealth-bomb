using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
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
	public GameObject customMappingPanel;

	public GameObject xboxControllerDisplay;

	public GameObject pcControllerDisplay;

	public bool newVersion = true;

	private GameObject btn;
	private Button b;


	PlayerIndex player1 = PlayerIndex.One;
	bool playerSet;
	GamePadState currentState;
	GamePadState prevState;
	GamePadState[] currentStates;
	GamePadState[] prevStates;
	PlayerIndex[] players;
	private int currentMainMenuButton;
	private int currentSelectionMenuButton;
	private int currentRemappingMenuButton;
	private int currentCustomMappingButton;

	// xbox controller variables
	public float controllerStickDeadZone = 0.5f; // keep this between 0 and 1.
	public float cooldown = 10.0f;
	private float timer;
	private bool hasMoved;
	private bool hasClicked;

	public int getCurrentPanel()
	{ return menu; }

	private void Start()
	{
		setMenu(1);
		playerSet = false;
		currentMainMenuButton = 1;
		CheckForAndSkipDeactivatedButtons(false);
		mainMenuButtons(currentMainMenuButton);
		currentSelectionMenuButton = 2;
		currentRemappingMenuButton = 1;
		timer = cooldown;   // 7.5f
		hasMoved = false;
		InputManager.inputManager.UseMouseAndKeyboardForFirstDisconnectedPlayer(false);
		currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		players = new PlayerIndex[4];

		for (int i = 0; i < 4; i++) {
			players[i] = (PlayerIndex) i;
		}
	}

	void Update() {
		// Debug.Log("cooldown = " + cooldown);

		if (newVersion)
			ControllerNavigationNewVersion();
		else
			ControllerNavigationOldVersion();
	}

	// rough version of being able to navigate main menu with more than 1 controller
	// NOTE: might be modified/replaced later to make it more clean (if menu controls are integrated with input manager)
	private void ControllerNavigationNewVersion() {
		// loops through all player indexes (input)
		for (int i = 0; i < players.Length; i++) {
			prevStates[i] = currentStates[i];
			currentStates[i] = GamePad.GetState(players[i]);

			if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
			// assuming cooldown is finished, allow input from controller
				if (timer >= cooldown) {
					hasMoved = false;
					// makes sure controller is connected and there is no other input since cooldown finished
					if (currentStates[i].IsConnected && !hasMoved) {
						if (mainMenuPanel.activeSelf == true) {
							if (currentStates[i].ThumbSticks.Left.Y > controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
								currentMainMenuButton--;
								CheckAndMoveCursorToBottom();
								CheckForAndSkipDeactivatedButtons(true);
								mainMenuButtons(currentMainMenuButton);
								hasMoved = true;
							}
							else if (currentStates[i].ThumbSticks.Left.Y < -controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
								currentMainMenuButton++;
								CheckAndMoveCursorToTop();
								CheckForAndSkipDeactivatedButtons(false);
								mainMenuButtons(currentMainMenuButton);
								hasMoved = true;
							}
						}

						if (selectionMenuPanel.activeSelf == true) {
							if (currentStates[i].ThumbSticks.Left.X > 0) {
								currentSelectionMenuButton++;
								if (currentSelectionMenuButton > 3) {
									currentSelectionMenuButton = 2;
								}
								selectionMenuButtons(currentSelectionMenuButton);
								hasMoved = true;
							}
						}

						if (remappingMenuPanel.activeSelf == true)
						{

							// update button pos
							if (currentStates[i].ThumbSticks.Left.Y < 0.0f)
							{
								currentRemappingMenuButton++;
								hasMoved = true;
							}
							else if (currentStates[i].ThumbSticks.Left.Y > 0f)
							{
								currentRemappingMenuButton--;
								hasMoved = true;
							}
							
							// fix outbound pos
							if (currentRemappingMenuButton > 5)
							{
								currentRemappingMenuButton = 1;
							}
							else if (currentRemappingMenuButton < 1)
							{
								currentRemappingMenuButton = 5;
							}
							
							remappingMenuButtons(currentRemappingMenuButton);
							
						}

						if (hasMoved) 
						{
							timer = 0.0f;
							timer += 1.0f * Time.deltaTime;
						}
					}
				}

				// NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!
				// getCurrentPanel() (or menu) needs to be <= 1, indicating that we are currently in the main menu, or we pressed quit while in the Unity editor.
				if (currentStates[i].Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() == 1) {
					Debug.Log("clicking");
					b.onClick.Invoke();
				}
				// Debug.Log("current main menu button: " + currentMainMenuButton);
			}
		}

		if (hasMoved && timer < cooldown) 
			timer += 1.0f * Time.deltaTime;
	}

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
	public void setMenu(int m)
	{ 
		menu = m; 
		switch(m)
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
                if(AudioPanel)
                    AudioPanel.SetActive(false);
				mainMenuButtons(1);
				currentMainMenuButton = 1;
				// Debug.Log("exiting main menu display");
				break;
			
			//Character Selection
			case 2:
				// Debug.Log("to selection");
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				remappingMenuPanel.SetActive(false);
				customMappingPanel.SetActive(false);
				selectionMenuButtons(2);
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
				remappingMenuButtons(1);
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
				customMappingButtons(0);
				currentCustomMappingButton = 0;
				break;
				
		}
	}

    public void OpenAudioPanel() //things would be easier to read as seperate functions
    {
        mainMenuPanel.SetActive(false);
        AudioPanel.SetActive(true);
        selectionMenuPanel.SetActive(false);
        remappingMenuPanel.SetActive(false);
    }

    public void SetVolume()
    {

    }

	public void SelectControllerDisplay(int i) {
		if (selectionMenuPanel.activeSelf == true) {
			switch (i) {
				case 0:
					xboxControllerDisplay.SetActive(true);
					pcControllerDisplay.SetActive(false);
					break;
				case 1:
					xboxControllerDisplay.SetActive(false);
					pcControllerDisplay.SetActive(true);
					break;
			}
		}
	}

	public void ResetControllerDisplay(bool doesNothing) {
		xboxControllerDisplay.SetActive(true);
		pcControllerDisplay.SetActive(false);
	}


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
		Debug.Log("main menu button selected: " + btn.name);
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
		b = btn.GetComponent<Button>();
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
