using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	/*
	int menu represents the current menu
	0 - quit
	1 - main menu
	2 - prep menu
	3 - game
	 */
	private static int menu = 0;
	
	//GameObject reference to menu panels
	public GameObject mainMenuPanel;
	public GameObject selectionMenuPanel;
	public GameObject settingMenuPanel;

	public int getCurrentPanel()
	{ return menu; }

	private void Start()
	{
		setMenu(1);
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
				break;
			
			//Game Preperation
			case 2:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				settingMenuPanel.SetActive(false);
				break;

			//Setting Menu
			case 3:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(true);
				break;
		}
	}

}
