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
	4 - settiong : player button mapping	
	 */
	private static int menu = 0;
	
	//GameObject reference to menu panels
	public GameObject mainMenuPanel;
	public GameObject selectionMenuPanel;
	public GameObject settingMenuPanel;
    public GameObject AudioPanel;
	public GameObject customMappingPanels;

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
	private int currentSettingsMenuButton;

	// xbox controller variables
	public float controllerStickDeadZone = 0.5f; // keep this between 0 and 1.
	public float cooldown = 10.0f;
	private float timer;
	private bool hasMoved;

	public int getCurrentPanel()
	{ return menu; }

	private void Start()
	{
		setMenu(1);
		playerSet = false;
		currentMainMenuButton = 1;
		currentSelectionMenuButton = 2;
		currentSettingsMenuButton = 2;
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
								if (currentMainMenuButton < 1) {
									currentMainMenuButton = mainMenuPanel.transform.childCount - 1;
								}
								mainMenuButtons(currentMainMenuButton);
								hasMoved = true;
							}
							else if (currentStates[i].ThumbSticks.Left.Y < -controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
								currentMainMenuButton++;
								if (currentMainMenuButton >= mainMenuPanel.transform.childCount) {
									currentMainMenuButton = 1;
								}
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
						if (hasMoved) 
						{
							timer = 0.0f;
							timer += 1.0f * Time.deltaTime;
						}
					}
				}

				// NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!!
				if (currentStates[i].Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() == 1) {
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
				Application.Quit();
				break;

			//Main menu
			case 1:
				mainMenuPanel.SetActive(true);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(false);
				customMappingPanels.SetActive(false);
                if(AudioPanel)
                    AudioPanel.SetActive(false);
				mainMenuButtons(1);
				currentMainMenuButton = 1;
				break;
			
			//Character Selection
			case 2:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				settingMenuPanel.SetActive(false);
				customMappingPanels.SetActive(false);
				selectionMenuButtons(2);
				currentSelectionMenuButton = 2;
				InputManager.inputManager.UseMouseAndKeyboardForFirstDisconnectedPlayer(false);
				break;

			//Setting Menu
			case 3:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(true);
				customMappingPanels.SetActive(false);
				settingMenuButtons(3);
				currentSettingsMenuButton = 3;
				break;
			case 4: //players mapping
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(false);
				customMappingPanels.SetActive(true);
				break;
				
		}
	}

    public void OpenAudioPanel() //things would be easier to read as seperate functions
    {
        mainMenuPanel.SetActive(false);
        AudioPanel.SetActive(true);
        selectionMenuPanel.SetActive(false);
        settingMenuPanel.SetActive(false);
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


	/*
	the three functions below will be called when the corsponding panel is active
	to enable the button highlight for controller
	 */
	private void mainMenuButtons( int i )
	{
		btn = mainMenuPanel.transform.GetChild(i).gameObject;
		_buttonSelect();
	}

	private void selectionMenuButtons( int i )
	{
		btn = selectionMenuPanel.transform.GetChild(i).gameObject;
		_buttonSelect();
	}

	private void settingMenuButtons( int i )
	{
		btn = settingMenuPanel.transform.GetChild(i).gameObject;
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
