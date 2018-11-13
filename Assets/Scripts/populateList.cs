using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class populateList : MonoBehaviour {

	public GameObject IM;
	public Dropdown LeftStick;
	public Dropdown RightStick;
	public Dropdown LeftBumper;
	public Dropdown LeftTrigger;	
	public Dropdown RightBumper;
	public Dropdown RightTrigger;

	private InputManager im;

	private Dictionary<string, string> sticks = new Dictionary<string, string>();
	private Dictionary<string, string> buttons = new Dictionary<string, string>();
	

	enum Buttons
	{ Attack,Dodge, Jump }

	enum Sticks
	{ Move, Aim}

	private void Awake()
	{

		im = IM.GetComponent<InputManager>();

		sticks.Add("left", "move");
		sticks.Add("right", "aim");

		buttons.Add("leftBumper", "jump");
		buttons.Add("leftTrigger", "dodge");
		buttons.Add("rightBumper", "attack");
		buttons.Add("rightTrigger", "attack");

		populateStickLists();
		
		// im.mapInput("move", "left");
		// im.mapInput("move", "right");
	}

	public void leftStickChange(int i) 
	{
		if (i==0)
			im.mapInput("move", "left");
		else if (i==1)
			im.mapInput("aim", "left");
	}

	public void rightStickChange(int i)
	{
		if (i==0)
			im.mapInput("move", "right");
		else if (i==1)
			im.mapInput("aim", "right");
	}

	public void leftBumperChange(int i)
	{
		if (i==0)
		{
			buttons.Remove("leftBumper");
			buttons.Add("leftBumper", "attack");
		}
		else if (i==1)
		{
			buttons.Remove("leftBumper");
			buttons.Add("leftBumper", "dodge");
		}
		else if (i==2)
		{
			buttons.Remove("leftBumper");
			buttons.Add("leftBumper", "jump");
		}
		im.mapInput( buttons["leftBumper"], "leftBumper");
	}

	public void leftTriggerChange(int i)
	{
		if (i==0)
		{
			buttons.Remove("leftTrigger");
			buttons.Add("leftTrigger", "attack");
		}
		else if (i==1)
		{
			buttons.Remove("leftTrigger");
			buttons.Add("leftTrigger", "dodge");
		}
		else if (i==2)
		{
			buttons.Remove("leftTrigger");
			buttons.Add("leftTrigger", "jump");
		}
		im.mapInput( buttons["leftTrigger"], "leftTrigger");
	}

	public void rightBumperChange(int i)
	{
		if (i==0)
		{
			buttons.Remove("rightBumper");
			buttons.Add("rightBumper", "attack");
		}
		else if (i==1)
		{
			buttons.Remove("rightBumper");
			buttons.Add("rightBumper", "dodge");
		}
		else if (i==2)
		{
			buttons.Remove("rightBumper");
			buttons.Add("rightBumper", "jump");
		}
		im.mapInput( buttons["rightBumper"], "rightBumper");	
	}

	public void rightTriggerChange(int i)
	{
		if (i==0)
		{
			buttons.Remove("rightTrigger");
			buttons.Add("rightTrigger", "attack");
		}
		else if (i==1)
		{
			buttons.Remove("rightTrigger");
			buttons.Add("rightTrigger", "dodge");
		}
		else if (i==2)
		{
			buttons.Remove("rightTrigger");
			buttons.Add("rightTrigger", "jump");
		}
		im.mapInput( buttons["rightTrigger"], "rightTrigger");
	}

	private void populateStickLists()
	{
		string[] sticks = Enum.GetNames(typeof(Sticks));
		List<string> s = new List<string>(sticks);
		LeftStick.AddOptions(s);
		RightStick.AddOptions(s);

		string[] buttons = Enum.GetNames(typeof(Buttons));
		List<string> s2 = new List<string>(buttons);
		LeftBumper.AddOptions(s2);
		LeftTrigger.AddOptions(s2);
		RightBumper.AddOptions(s2);
		RightTrigger.AddOptions(s2);
	}

	
}
