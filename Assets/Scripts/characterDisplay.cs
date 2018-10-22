using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterDisplay : MonoBehaviour {

	private List<Transform> characterList;
	private Transform character;

	/*
	assume total 4 characters to choose, default is the 1st
	 */
	public int choice = 1;
	private int choiceLimit = 4;

	/*
	add all children(chracters) to the list and enable the 1st to display
	 */
	private void Start()
	{
		characterList = new List<Transform>();
		for (int i= 0; i < transform.childCount; i++)
		{
			character = transform.GetChild(i);
			characterList.Add(character);
			character.gameObject.SetActive(i == 0);
		}
	}

	/*
	update character display by up and down input
	 */
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Debug.Log("UP!");
			choice++;
			if (choice > 4f)
				choice %= choiceLimit;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Debug.Log("Down!");
			choice--;
			if (choice <= 0f)
				choice = choiceLimit;
			// choice = Mathf.Abs(choice);
		}

		Debug.Log("choice: " + choice);
		enableCharacterModel(choice-1);
	}

	/*
	enable model at index chara
	 */
	public void enableCharacterModel(int chara)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			character = transform.GetChild(i);
			character.gameObject.SetActive(i == chara);
		}
	}

	/*
	for OK button on selection
	 */
	public void confirmSelection()
	{
		Debug.Log("Player one selects character" + choice + "!");
	}
}
