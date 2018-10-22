using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelection : MonoBehaviour {

	// private Transform uielement;
	public Text t;
	public Text readyT;

	void Start()
	{
		// uielement = transform.GetChild(4);
		// t = uielement.gameObject.GetComponent<Text>();
	}

	public void playerConnected()
	{
		// uielement.gameObject.SetActive(true);
		t.text = "Press A";
	}

	public void playerDisconnected()
	{
		t.text = "Disconnected";
	}

	public void playerIsRead()
	{
		t.text = "Ready";
	}

	public void playerIsNotReady()
	{
		t.text = "Not Ready";
	}

	public void gameIsReady()
	{
		readyT.text = "Game is ready to start";
	}

	public void gameIsNotReady()
	{
		readyT.text = "Not Enough Players";
	}

}
