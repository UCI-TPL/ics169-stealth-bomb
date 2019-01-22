using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtable
{
    /// <summary>
    /// Deals damage to this object
    /// </summary>
    /// <param name="damageDealer"> Player the damage is originating from </param>
    /// <param name="amount"> Amount of damage to deal </param>
    /// <returns> Something </returns>
    float Hurt(Player damageDealer, float amount);
}
