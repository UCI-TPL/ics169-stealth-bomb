using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Power-up/Power-up", order = 1)]
public class PowerupData : ScriptableObject {
    
    public static float duration = 10; // Buff duration for all powerups

    public new string name;
    public Sprite image;
    public string description;
    
    // Holds a list of all modifiers in the powerup
    [SerializeField]
    public List<PlayerStats.Modifier> modifiers;

    public Powerup instance;

    private void OnEnable() {
        if (modifiers == null) // Initialize list of modifiers if not already
            modifiers = new List<PlayerStats.Modifier>();
        instance.data = this; // Set powerup data to this
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
