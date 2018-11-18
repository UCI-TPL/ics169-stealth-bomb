using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
[CreateAssetMenu(fileName = "New Special Move", menuName = "Moves", order = 0)]
*/


public abstract class SpecialMoveData : WeaponData//<SpecialMove>
{

    public float moveDuration; //how long is the move

    public MoveType moveType;

    public enum MoveType
    {
        DodgeDash, DodgeRoll, Summon, Shield
    }

    /*
    public override Weapon NewInstance(Player player)
    {
        Weapon copy = instance.DeepCopy(this, player);
        return copy;
    }
    */


}

