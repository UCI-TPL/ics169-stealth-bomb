using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Item/Power-up", order = 1)]
public class PowerupData : ItemData {
    
    public static float duration = 10; // Buff duration for all powerups
    
    // Holds a list of all modifiers in the powerup
    [SerializeField]
    public List<PlayerStats.Modifier> modifiers;

    public Powerup instance;

    private void OnEnable() {
        type = ItemData.Type.Powerup;
        if (modifiers == null) // Initialize list of modifiers if not already
            modifiers = new List<PlayerStats.Modifier>();
        if (instance == null)// Set powerup data to this if instance is not yet created
            instance = new Powerup(this);
        instance.data = this;
    }

    // Add modifier to the powerup
    public void AddModifier(string name, float value) {
        modifiers.Add(new PlayerStats.Modifier(name, value));
    }

    // Remove modifier from the powerup
    public void RemoveModifier(int index) {
        modifiers.RemoveAt(index);
    }

    public virtual Powerup NewInstance(Player player, bool isPermenant = false) {
        return instance.Clone(player, isPermenant);
    }
}
