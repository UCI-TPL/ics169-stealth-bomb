using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	/*
	stage is an int representing the current stage
	-1 - quit
	0 - main menu
	1 - prep menu
	2 - game
	 */
	private static int stage = 0;
	
	private static int numOfPlayers = 2;
	
	//prep
	public Text num;
	public Slider s;

	public GameObject mainMenuPanel;
	public GameObject prepMenuPanel;

	public int getMenuStage()
	{ return stage; }

	public int getPlayers()
	{ return numOfPlayers; }


	private void Start()
	{
		setMenu(0);
	}

	/*
	public methods for external usage
	 */
	public void setMenu(int s)
	{ 
		stage = s; 

		//Quit
		if (stage == -1)
		{
			Application.Quit();
		}

		//Main Menu
		else if (stage == 0)
		{
			mainMenuPanel.SetActive(true);
			prepMenuPanel.SetActive(false);
		}

		//Game Preperation
		else if (stage == 1)
		{
			mainMenuPanel.SetActive(false);
			prepMenuPanel.SetActive(true);
			updateDisplayNum();
		}

		//Let Us Play Together!
		else if (stage == 2)
		{
			mainMenuPanel.SetActive(false);
			prepMenuPanel.SetActive(false);
		}
	}

	public void setPlayers()
	{ 
		numOfPlayers = (int)s.value; 
	}

	public void updateDisplayNum()
	{
		num.text = s.value.ToString();
		setPlayers();
	}

}
