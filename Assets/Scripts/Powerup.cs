using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Power up/Power up", order = 1)]
public class Powerup : ScriptableObject {
    
    public static float duration = 10; // Buff duration for all powerups

    public new string name;
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

    public virtual void OnAdd(Player player) { }

    public virtual void OnUpdate(Player player) { }

    public virtual void OnRemove(Player player) { }
}
