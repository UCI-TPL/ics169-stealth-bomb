using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Power-up/Power-up", order = 1)]
public class PowerupData : ScriptableObject {
    
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

    public virtual Powerup NewInstance(Player player, bool isPermenant = false) {
        return new Powerup(this, player, isPermenant);
    }
}

public class Powerup {
    private PowerupData data;
    public float startTime;
    public float endTime;
    public float timeRemaining {
        get { return endTime - Time.time; }
    }
    public bool isPermenant {
        get { return float.IsPositiveInfinity(endTime); }
    }
    public virtual List<PlayerStats.Modifier> modifiers {
        get { return data.modifiers; }
    }
    internal Player player;

    // List of functions to be called every update on a player
    public UnityEvent onUpdate = new UnityEvent();

    internal Powerup(Player player, bool isPermenant = false) {
        this.player = player;
        startTime = Time.time;
        endTime = isPermenant ? float.PositiveInfinity : Time.time + PowerupData.duration;
    }

    public Powerup(PowerupData data, Player player, bool isPermenant = false) {
        this.data = data;
        this.player = player;
        startTime = Time.time;
        endTime = isPermenant ? float.PositiveInfinity : Time.time + PowerupData.duration;
    }

    internal void AddUpdate(UnityAction updateMethod) {
        onUpdate.AddListener(updateMethod);
    }
}
