using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Powerup : ScriptableObject {
    
    public static float duration = 10; // Buff duration for all powerups

    public string name;
    public Texture2D image;
    public string description;
    
    // Holds a list of all modifiers in the powerup
    [SerializeField]
    public List<PlayerStats.Modifier> modifiers;

    private void OnEnable() {
        if (modifiers == null) // Initialize list of modifiers if not already
            modifiers = new List<PlayerStats.Modifier>();
    }

    // Add modifier to the powerup
    public void AddModifier(string name, float value) {
        modifiers.Add(new PlayerStats.Modifier(name, value));
    }

    // Remove modifier from the powerup
    public void RemoveModifier(int index) {
        modifiers.RemoveAt(index);
    }

    // On going effects of the powerup when it is on a player
    public virtual void OnUpdate() { }
}
