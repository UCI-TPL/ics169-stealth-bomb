using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Item/Power-up", order = 0)]
public class PowerupData : ItemData {
    
    public static float duration = 10; // base duration for all powerups
    
    // Holds a list of all buffs in the powerup
    [SerializeField]
    public BuffData buffData;

    private void OnEnable() {
        type = ItemData.Type.Powerup;
    }

    public override void Use(Player player) {
        player.AddBuff(buffData.Instance(duration, this));
    }
}
