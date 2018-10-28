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
	0 - quit
	1 - main menu
	2 - selection menu
	3 - setting menu
	 */
	private static int menu = 0;
	
	//GameObject reference to menu panels
	public GameObject mainMenuPanel;
	public GameObject selectionMenuPanel;
	public GameObject settingMenuPanel;

	private GameObject btn;
	private Button b;


	PlayerIndex player1 = PlayerIndex.One;
	bool playerSet;
	GamePadState currentState;
	GamePadState prevState;
	private int currentMainMenuButton;
	private int currentSelectionMenuButton;
	private int currentSettingsMenuButton;

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
		timer = 7.5f;
		hasMoved = false;
	}

	void Update() {
		// code modified from 3rd party script: XInputTestCS.cs
		// if (!playerSet || !prevState.IsConnected) {
		// 	for (int i = 0; i < 4; i++) {
		// 		PlayerIndex testPlayer = (PlayerIndex) i;
		// 		//player = (PlayerIndex) i;
		// 		GamePadState testState = GamePad.GetState(testPlayer);
		// 		if (testState.IsConnected) {
		// 			playerSet = true;
		// 			player1 = testPlayer;
		// 		}
		// 	}
		// }
		// prevState = currentState;
		// currentState = GamePad.GetState(player1);
		// end of borrowed/modified code

		prevState = currentState;
		currentState = GamePad.GetState(player1);

		// for now, only player 1 should be able to control the main menu.
		if (currentState.IsConnected && timer >= cooldown) {
			if (mainMenuPanel.activeSelf == true) {
				if (currentState.ThumbSticks.Left.Y > 0.0f /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
					currentMainMenuButton--;
					if (currentMainMenuButton < 1) {
						currentMainMenuButton = mainMenuPanel.transform.childCount - 1;
					}
					mainMenuButtons(currentMainMenuButton);
					hasMoved = true;
				}
				else if (currentState.ThumbSticks.Left.Y < 0.0f /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
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
		Debug.Log("current main menu button: " + currentMainMenuButton);
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
				mainMenuButtons(1);
				currentMainMenuButton = 1;
				break;
			
			//Character Selection
			case 2:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				settingMenuPanel.SetActive(false);
				selectionMenuButtons(2);
				currentSelectionMenuButton = 2;
				break;

			//Setting Menu
			case 3:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(true);
				settingMenuButtons(2);
				currentSettingsMenuButton = 2;
				break;
		}
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

}
