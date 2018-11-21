using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class populateList : MonoBehaviour {

	public GameObject IM;
	public Dropdown Move;
	public Dropdown Aim;
	public Dropdown Attack;
	public Dropdown Dodge;	
	public Dropdown Jump;
	public int playerID;
	
	private InputManager im;


	/*
	Dictionary Key: button, Value: action
	 */
	private Dictionary<string, string>[] mapMatrix= new Dictionary<string, string>[]
	{
		new Dictionary<string, string>(),
		new Dictionary<string, string>(),
		new Dictionary<string, string>(),
		new Dictionary<string, string>()
	};
	

	// enum Buttons
	// { Attack,Dodge, Jump }

	// enum Sticks
	// { Move, Aim }

	enum  Sticks 
	{ LeftStick, RightStick }

	enum Buttons
	{ A, B, X, Y, LeftBumper, RightBumper, LeftTrigger, RightTrigger }

	private void Start()
	{
		im = IM.GetComponent<InputManager>();

		for (int i=0; i<4; ++i)
		{
			mapMatrix[i].Add("move", "leftStick");
			mapMatrix[i].Add("aim", "righStick");
			mapMatrix[i].Add("jump", "leftBumper");
			mapMatrix[i].Add("dodge", "leftTrigger");
			mapMatrix[i].Add("attack", "rightBumper");
		}
		//populateControlLists();
		
	}


	/*
	button change functions
	track player's customization in dictionary (not permanently)
	 */
	#region button change
	public void moveCtl( int i )
	{
		im.removeInputMapping("move", mapMatrix[playerID]["move"], playerID);
		mapMatrix[playerID].Remove("move");
		changeStickCtl("move", i);
		im.mapInput("move", mapMatrix[playerID]["move"], playerID);
	}


	public void aimCtl(int i) 
	{
		im.removeInputMapping("aim", mapMatrix[playerID]["aim"], playerID);
		mapMatrix[playerID].Remove("aim");
		changeStickCtl("aim", i);
		im.mapInput("aim", mapMatrix[playerID]["aim"], playerID);
	}

	public void attackCtl(int i)
	{
		im.removeInputMapping("attack", mapMatrix[playerID]["attack"], playerID);
		mapMatrix[playerID].Remove("attack");
		changeCtl("attack", i);
		im.mapInput("attack", mapMatrix[playerID]["attack"], playerID);
	}

	public void dodgeCtl(int i)
	{
		im.removeInputMapping("dodge", mapMatrix[playerID]["dodge"], playerID);
		mapMatrix[playerID].Remove("dodge");
		changeCtl("dodge", i);
		im.mapInput("dodge",mapMatrix[playerID]["dodge"], playerID);
	}

	public void jumpCtl(int i)
	{
		im.removeInputMapping("jump", mapMatrix[playerID]["jump"], playerID);
		mapMatrix[playerID].Remove("jump");
		changeCtl("jump", i);
		im.mapInput("jump", mapMatrix[playerID]["jump"], playerID);
	}

	private void changeStickCtl( string action, int i)
	{
		switch(i)
		{
			case 0:
				mapMatrix[playerID].Add(action, "leftStick");
				break;
			case 1:
				mapMatrix[playerID].Add(action, "rightStick");
				break;
		}
	}

	private void changeCtl( string action, int i )
	{
		switch(i)
		{
			case 0:
				mapMatrix[playerID].Add(action, "A");
				break;
			case 1:
				mapMatrix[playerID].Add(action, "B");
				break;
			case 2:
				mapMatrix[playerID].Add(action, "X");
				break;
			case 3:
				mapMatrix[playerID].Add(action, "Y");
				break;
			case 4:
				mapMatrix[playerID].Add(action, "leftBumper");
				break;
			case 5:
				mapMatrix[playerID].Add(action, "rightBumper");
				break;
			case 6:
				mapMatrix[playerID].Add(action, "leftTrigger");
				break;
			case 7:
				mapMatrix[playerID].Add(action, "rightTrigger");
				break;
		}
		
	}
	#endregion

	/*
	add enum sticks and buttons options to dropdown options
	 */
	private void populateControlLists()
	{
		string[] sticks = Enum.GetNames(typeof(Sticks));
		List<string> s = new List<string>(sticks);
		Move.AddOptions(s);
		Move.value = 0;
		Aim.AddOptions(s);
		Aim.value = 1;

		string[] buttons = Enum.GetNames(typeof(Buttons));
		List<string> s2 = new List<string>(buttons);
		Attack.AddOptions(s2);
		Attack.value = 5;
		Dodge.AddOptions(s2);
		Dodge.value = 6;
		Jump.AddOptions(s2);
		Jump.value = 4;
	}

	public void playerIs(int i)
	{ playerID = i; }

	
}
