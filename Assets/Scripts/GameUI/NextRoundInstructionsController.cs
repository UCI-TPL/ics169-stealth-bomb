using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextRoundInstructionsController : MonoBehaviour {

	private Text instructions;
	
	public string nextRoundStr = "Press Start to begin next round";
	public string returnMenuStr = "Press Start to return to main menu";

	void Awake() {
		instructions = gameObject.GetComponent<Text>();
	}

	// Use this for initialization
	void Start () {
		ChangeToRoundInstructions();
	}
	
	// Update is called once per frame
	void Update () {
		// Debug.Log("Number of winners = " + GameManager.instance.Winners.Count);
		if (ProgressScreenUI.Instance.GameWon == true) 
			ChangeToMenuInstructions();
		else 
			ChangeToRoundInstructions();
	}

	private void ChangeToRoundInstructions() {
		instructions.text = nextRoundStr;
	}

	private void ChangeToMenuInstructions() {
		instructions.text = returnMenuStr;
	}
}
