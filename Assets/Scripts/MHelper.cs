using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MHelper : MonoBehaviour {

	private StageManager stageManager;

	private void Start()
	{
		stageManager = FindObjectOfType<StageManager>();
		if (stageManager == null)
		{
			Debug.Log("Could not find stageManager");
		}
	}

	public void refSetStage(int s)
	{
		stageManager.setStage(s);
	}

	public void refSetPlayers(int p)
	{
		stageManager.setPlayers(p);
	}
}
