using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displaySliderValue : MonoBehaviour {
		
	public Text displayValue;
	public Slider s;

	private StageManager stageManager;
	private void Start()
	{
		stageManager = FindObjectOfType<StageManager>();
		if (stageManager == null)
		{
			Debug.Log("Could not find stageManager");
		}
	}

	public void Update()
	{
		displayValue.text = s.value.ToString();

	}

	public void refSetPlayers()
	{
		stageManager.setPlayers((int)s.value);
	}
}
