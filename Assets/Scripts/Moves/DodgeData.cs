﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dodge", menuName = "Moves/Dodge")]
public class DodgeData : SpecialMoveData    //should be named DodgeDashData
{
    
    public DodgeData() { moveType = MoveType.DodgeDash; }
    
    [Tooltip("How many times DodgeSpeed should be faster than normal speed")]
    public float SpeedMultiplier;

    [Tooltip("Move while dashing")]
    public bool MoveDuringDash;

    [Tooltip("Slow down the dodge as it goes on to make it look real smooth")]
    public float DodgeDeceleration;

    
    public override Weapon NewInstance(Player player)
    {
        if (moveType == SpecialMoveData.MoveType.DodgeDash) //make one for every type of SpecialMove
            return  (new DodgeDash(this, player));
        //else if (moveType == SpecialMoveData.MoveType.DodgeRoll)
        //    return  (new DodgeRoll(this, player));
        return null;
    }
    
}


