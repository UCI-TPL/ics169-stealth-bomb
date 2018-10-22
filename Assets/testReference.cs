using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testReference : MonoBehaviour {

	private characterSelection selectionOP;

	public GameObject playerObject;

	private void Start()
	{
		selectionOP = playerObject.GetComponent<characterSelection>();
		
		selectionOP.playerIs(0);
		selectionOP.playerDisconnected();

		selectionOP.playerIs(2);
		selectionOP.playerIsReady();

		selectionOP.gameIsReady();
	}
}
