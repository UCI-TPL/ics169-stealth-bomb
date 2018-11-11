using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData : ScriptableObject{

    public Buff instance;

    private void OnEnable() {
        if (instance == null)// Set powerup data to this if instance is not yet created
            instance = new Buff();
    }

    // Add modifier to the powerup
    public void AddModifier(string name, float value) {
        instance.modifiers.Add(new PlayerStats.Modifier(name, value));
    }

    // Remove modifier from the powerup
    public void RemoveModifier(int index) {
        instance.modifiers.RemoveAt(index);
    }

    public virtual Buff NewInstance(Player player, bool isPermenant = false) {
        return instance.Clone(player, isPermenant);
    }
}
