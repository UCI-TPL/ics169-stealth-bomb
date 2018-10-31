using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Health Potion", menuName = "Item/Item/Health Potion", order = 1)]
public class HealthPotionData : ItemData {

    public float amount;

    public override void Use(Player player) {
        player.Heal(amount);
    }
}
