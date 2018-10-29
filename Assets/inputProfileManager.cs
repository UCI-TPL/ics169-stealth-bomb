using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputProfileManager : MonoBehaviour {

	List<string> moveOpList = new List<string>{ "ThumbSticks.Left", "ThumbSticks.Right" };
	List<string> attackOpList = new List<string>{ "Buttons.X", "Buttons.Y", "Buttons.A", "Buttons.B",
												 "Buttons.RightShoulder", "Buttons.LeftShoulder",
												 "Trigger.Right", "Trigger.Left"};
	
	public Dropdown moveOpDp;
	public Dropdown attackOpDp;

	private Hashtable opDic = new Hashtable();

	private void Start()
	{
		moveOpDp.AddOptions(moveOpList);
		attackOpDp.AddOptions(attackOpList);
		initialProfile();
	}

	private void Update()
	{
		Debug.Log("move: " + opDic["Move"] + " attack: " + opDic["Attack"]);
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
