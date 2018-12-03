using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StackingBuff : Buff {

    public StackingBuff() : base() { }

    private string counterName;
    private PlayerStats.Modifier counter;

    private static Dictionary<BuffData, Dictionary<Player, List<Trigger>>> staticTriggers = new Dictionary<BuffData, Dictionary<Player, List<Trigger>>>();

    // Contructor used to clone instance of powerup
    private StackingBuff(float duration, object source) : base(duration, source) { }

    public override void Equip(Player player) {
        foreach (PlayerStats.Modifier m in Modifiers) // Add power-up's modifiers to stats
            player.stats.AddModifier(m);
        player.stats.AddModifier(counter);
        if (player.stats.GetStat(counterName) == 1) {

            if (!staticTriggers[buffData].ContainsKey(player))
                staticTriggers[buffData].Add(player, Triggers);

            AddTriggers(player, staticTriggers[buffData][player]);
        }
    }

    public override void Unequip(Player player) {
        foreach (PlayerStats.Modifier m in Modifiers) // Remove all modifiers granted by this powerup
            player.stats.RemoveModifier(m);
        player.stats.RemoveModifier(counter);
        if (player.stats.GetStat(counterName) <= 0) {
            RemoveTriggers(player, staticTriggers[buffData][player]);

            staticTriggers[buffData].Remove(player);
        }
    }

    // Create a deep copy of this powerup instance. Used for when adding a new powerup to a player
    public override Buff DeepCopy(BuffData buffData, float duration, object source) {
        StackingBuff copy = new StackingBuff(duration, source);
        copy.buffData = buffData;
        copy.Modifiers = Modifiers;

        if (!staticTriggers.ContainsKey(buffData))
            staticTriggers.Add(buffData, new Dictionary<Player, List<Trigger>>());
        foreach (Trigger t in Triggers) {
            copy.Triggers.Add(t.DeepCopy(copy));
        }
        copy.counterName = ((StackingBuffData)buffData).counterName;
        copy.counter = new PlayerStats.Modifier(copy.counterName, 1);
        return copy;
    }
}
