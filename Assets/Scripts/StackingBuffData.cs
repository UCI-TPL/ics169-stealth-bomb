using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stacking Buff", menuName = "Buff/Stacking Buff", order = 3)]
public class StackingBuffData : BuffData<StackingBuff> {

    public string counterName;
}
