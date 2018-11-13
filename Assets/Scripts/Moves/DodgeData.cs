﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dodge", menuName = "Moves/Dodge")]
public class DodgeData : SpecialMoveData
{
    public DodgeData() { type = MoveType.DodgeDash; }

    [Tooltip("How many times DodgeSpeed should be faster than normal speed")]
    public float SpeedMultiplier;

}

