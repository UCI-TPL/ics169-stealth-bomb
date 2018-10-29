using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class inputProfileManager : MonoBehaviour {

	List<string> moveOpList = new List<string>{ "ThumbSticks.Left", "ThumbSticks.Right" };
	List<string> attackOpList = new List<string>{ "Buttons.X", "Buttons.Y", "Buttons.A", "Buttons.B",
												 "Buttons.RightShoulder", "Buttons.LeftShoulder",
												 "Trigger.Right", "Trigger.Left"};
	
	public Dropdown moveOpDp;
	public Dropdown attackOpDp;

	private Hashtable opDic = new Hashtable();
	private Hashtable opPrevStage = new Hashtable();
	private Hashtable opCurrentStage = new Hashtable();

	private GamePadState currentState;

	private void Start()
	{
		moveOpDp.AddOptions(moveOpList);
		attackOpDp.AddOptions(attackOpList);
		initialProfile();
		currentState = GamePad.GetState(0);
	}

	private void Update()
	{
		currentState = GamePad.GetState(0);
		// Debug.Log("x: " + getInputValue("MoveX") + " y: " + getInputValue("MoveY"));
		Debug.Log(getInputValue("Attack"));
	}

	/*
	return the value of corrsponding operation
	for example. getInputValue("MoveX") will return the value of movement on x-axis
					based on the setted input profile. if player sets ThumbSticks.Left for movement, 
					it will only return ThumbSticks.Left's X-axis's value
	MoveX, MoveY, Attack
	 */
	public float getInputValue(string op)
	{
		string input;
		if (op == "MoveX")
		{
			input = (string)opDic["Move"] + ".X";
		}
		else if (op == "MoveY")
		{
			input = (string)opDic["Move"] + ".Y";
		}
		else
		{
			input = (string)opDic[op];
		}

		float value = 0f;
		switch(input)
		{
			case "ThumbSticks.Left.X":
				value = (float)currentState.ThumbSticks.Left.X;
				break;
			case "ThumbSticks.Left.Y":
				value = currentState.ThumbSticks.Left.Y;
				break;
			case "ThumbSticks.Right.X":
				value = currentState.ThumbSticks.Right.X;
				break;
			case "ThumbSticks.Right.Y":
				value = currentState.ThumbSticks.Right.Y;
				break;
			case "Buttons.X":
				if (currentState.Buttons.X == ButtonState.Pressed)
					value = 1f;
				break;
			case "Buttons.Y":
				if (currentState.Buttons.Y == ButtonState.Pressed)
					value = 1f;
				break;
			case "Buttons.A":
				if (currentState.Buttons.A == ButtonState.Pressed)
					value = 1f;
				break;
			case "Buttons.B":
				if (currentState.Buttons.B == ButtonState.Pressed)
					value = 1f;
				break;	
			case "Buttons.RightShoulder":
				if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
					value = 1f;
				break;
			case "Buttons.LeftShoulder":
				if (currentState.Buttons.LeftShoulder == ButtonState.Pressed)
					value = 1f;
				break;
			case "Triggers.Left":
				value = currentState.Triggers.Left;
				break;
			case "Triggers.Right":
				value = currentState.Triggers.Right;
				break;
		}
		return value;
	}

	public string getMoveOp()
	{
		return (string)opDic["Move"];
	}

	public string getAttackOp()
	{
		return (string)opDic["Attack"];
	}

	private void initialProfile()
	{
		switchMoveOp(0);
		switchAttackOp(0);
	}

	public void switchMoveOp( int i )
	{
		opDic["Move"] = moveOpList[i];
	}

	public void switchAttackOp( int i )
	{
		opDic["Attack"] = attackOpList[i];
	}
}
