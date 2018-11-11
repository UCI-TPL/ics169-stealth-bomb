using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Move", menuName = "Moves", order = 0)]
public abstract class SpecialMoveData : ScriptableObject
{
    public float moveDuration; //how long is the move
    public float cooldown; //how long till the move can be used again 
    public string description;
    public MoveType type;

    public enum MoveType
    {
        DodgeDash, Summon, Shield
    }


}

[CreateAssetMenu(fileName = "New Dodge", menuName = "Moves/Dodge", order = 0)]
public class DodgeData : SpecialMoveData
{
    public DodgeData() { type = MoveType.DodgeDash; }

    [Tooltip("How many times DodgeSpeed should be faster than normal speed")]
    public float SpeedMultiplier;

}

