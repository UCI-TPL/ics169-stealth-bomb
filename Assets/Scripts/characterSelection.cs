using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelection : MonoBehaviour {

	public GameObject players;
	public List<Text> playerOpList;
	public Text readyText;

	private int player = 0;

	private void Awake()
	{
		/*
		go through players and add each player's opinfo (Text) to the playerOpList
		 */
		playerOpList = new List<Text>();
		for (int i= 0; i < players.transform.childCount; i++)
		{
			Transform t = players.transform.GetChild(i).transform.GetChild(1);
			playerOpList.Add(t.GetComponent<Text>());
		}
		// playerIs(0);
		// playerConnected();

		// playerIs(1);
		// playerDisconnected();

		// playerIs(2);
		// playerIsNotReady();

		// playerIs(3);
		// playerIsReady();

		// gameIsNotReady();
	}

	/*
	playerIs() set player to p:
	0 - player 1
	1 - player 2
	2 - player 3
	3 - player 4
	
	then call any followed public function to change corsponding UI

	 */
	public void playerIs(int p)
	{
		player = p;
		// Debug.Log("player is: " + player);
	}

	public void playerDisconnected()
	{
		playerOpList[player].text = "Disconnected";
	}

	public void playerConnected()
	{
		playerOpList[player].text = "Press A to join";
	}

	public void playerIsReady()
	{
		playerOpList[player].text = "Ready";
	}

	public void playerIsNotReady()
	{
		playerOpList[player].text = "Not Ready";
	}

	public void gameIsReady()
	{
		// readyText.text = "Game is ready! Press <quad material=1 size=20 x=0.1 y=0.1 width=0.5 height=0.5/>";
		readyText.text = "Game is ready! Press Start";
	}

	public void gameIsNotReady()
	{
		readyText.text = "Not Enough Players";
	}

}
