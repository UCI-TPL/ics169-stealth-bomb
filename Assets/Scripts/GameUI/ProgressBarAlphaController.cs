using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarAlphaController : MonoBehaviour {

	public Image background;
	public Image fill;
	[Range(0,1)]
	public float alphafadeOutValue = 0.5f;

	void Awake() {
		background.color = new Color(background.color.r, background.color.g, background.color.b, 1.0f);
		fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, 1.0f);
	}

	// Use this for initialization
	void Start () {
		// background.color = new Color(background.color.r, background.color.g, background.color.b, 1.0f);
		// fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, 1.0f);
	}
	
	public void FadeOutProgressBar() {
		background.color = new Color(background.color.r, background.color.g, background.color.b, alphafadeOutValue);
		fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, alphafadeOutValue);
	}
}
