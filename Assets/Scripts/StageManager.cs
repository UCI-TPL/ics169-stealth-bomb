using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour {

	private static StageManager instance;

	/*
	stage is an int representing the current stage
	-1 - quit
	0 - main menu
	1 - prep menu
	2 - game
	 */
	private static int stage = 0;
	private static int numOfPlayers = 2;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	private void Update()
	{
		Debug.Log(getPlayers().ToString());
	}


	/*
	public methods for external usage
	 */

	public void setStage(int s)
	{ 
		stage = s; 
		setScene();
	}

	public void setPlayers(int p)
	{
		numOfPlayers = p;
	}

	public int getStage()
	{ return stage; }

	public int getPlayers()
	{ return numOfPlayers; }


	/*
	private methods for internal usage
	 */
	private void setScene()
	{
		if (stage == 0)
		{
			SceneManager.LoadScene("kyle_MainMenu");
		}
		else if (stage == 1)
		{
			SceneManager.LoadScene("kyle_PrepMenu");
		}
		else if (stage == 2)
		{
			SceneManager.LoadScene("kyle_tmpEmptyGameScene");
		}
		else if (stage == -1)
		{
			Application.Quit();
		}
	}

}
