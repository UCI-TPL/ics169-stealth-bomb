using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Summon", menuName = "Moves/Summon")]
public class SummonData : SpecialMoveData    //should be named DodgeDashData
{

    public SummonData() {  }

    [Tooltip("The object that needs to be summoned")]
    public GameObject summon;

    [Tooltip("How far away should the summon appear")]
    public float distanceFromPlayer;

    public override Weapon NewInstance(Player player)
    {
        if (moveType == SpecialMoveData.MoveType.Summon) //make one for every type of SpecialMove
            return (new SummonWall(this, player));
        return null;
    }

}