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
				mainMenuButtons(1);
				break;
			
			//Character Selection
			case 2:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(true);
				settingMenuPanel.SetActive(false);
				selectionMenuButtons(2);
				break;

			//Setting Menu
			case 3:
				mainMenuPanel.SetActive(false);
				selectionMenuPanel.SetActive(false);
				settingMenuPanel.SetActive(true);
				settingMenuButtons(2);
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
