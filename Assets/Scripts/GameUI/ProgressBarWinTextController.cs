using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarWinTextController : MonoBehaviour {
	
	// should already be assigned in prefab, but if not, assign it in inspector.
	public Text WinText;

	public bool alreadySetUp;

	void Awake() {
		if (WinText == null) {
			WinText = gameObject.GetComponentInChildren<Text>();
		}
	}

	// Use this for initialization
	void Start () {
		// WinText.enabled = false;
		alreadySetUp = false;
	}

	public void SetupWinText(Player player) {
		WinText.text = "Player " + (player.playerNumber + 1) + " Wins!";
		WinText.color = player.Color;
		alreadySetUp = true;
	}

	public void TurnOnWinText() {
		WinText.enabled = true;
	}

	public void TurnOffWinText() {
		WinText.enabled = false;
	}
}
