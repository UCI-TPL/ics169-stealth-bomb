﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
[CreateAssetMenu(fileName = "New Special Move", menuName = "Moves", order = 0)]
*/


public abstract class SpecialMoveData : WeaponData//<SpecialMove>
{

    public float moveDuration; //how long is the move
    public float cooldown; //how long till the move can be used again 

    public MoveType moveType;

    public enum MoveType
    {
        DodgeDash, DodgeRoll, IceWall, Summon, Shield
    }

    /*
    public override Weapon NewInstance(Player player)
    {
        Weapon copy = instance.DeepCopy(this, player);
        return copy;
    }
    */


}

