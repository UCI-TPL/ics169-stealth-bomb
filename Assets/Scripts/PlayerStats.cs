using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats {
    
    public Player player { get; private set; }

    private Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

    public PlayerStats(Player player) {
        this.player = player;
        AddStat("move_speed", MoveSpeedStat); //blocks per second
        AddStat("air_speed", AirSpeedStat);  //percent of movespeed
        AddStat("jump_force", JumpForceStat); //blocks per jump (1,2,3)
        AddStat("max_health", MaxHealthStat);
        AddStat("dodge_time", DodgeTimeStat); //how long the dodge lasts
        AddStat("dodge_recharge", DodgeRechargeStat); //how long the player waits before dodging again
        AddStat("damage", DamageStat); // Base damage the player does
    }

    private readonly Stat MoveSpeedStat = new Stat(7.5f);
    private readonly Stat JumpForceStat = new Stat(1.5f);
    private readonly Stat MaxHealthStat = new Stat(100);
    private readonly Stat AirSpeedStat = new Stat(50);
    private readonly Stat DodgeTimeStat = new Stat(0.2f);
    private readonly Stat DodgeRechargeStat = new Stat(0.8f);
    private readonly Stat DamageStat = new Stat(1);

    public float moveSpeed {
        get { return MoveSpeedStat.Value; }
    }
    public float jumpForce {
        get { return JumpForceStat.Value; }
    }
    public float maxHealth {
        get { return MaxHealthStat.Value; }
    }
    public float airSpeed    {
        get { return (AirSpeedStat.Value / 100) * moveSpeed;  }
    }
    public float dodgeTime    {
        get { return DodgeTimeStat.Value; }
    }

    public float dodgeRecharge {
        get { return DodgeRechargeStat.Value; }
    }

    public float Damage {
        get { return DamageStat.Value; }
    }


    // Add a new stat
    private void AddStat(string name, float value) {
        stats.Add(name, new Stat(value));
    }

    // Add a new stat
    private void AddStat(string name, Stat stat) {
        stats.Add(name, stat);
    }

    // Attempts to grab a stat, if that stat does not exist display error
    public float GetStat(string name) {
        if (stats.ContainsKey(name))
            return stats[name].Value;
        Debug.LogWarning("Player " + player.playerNumber + " does not contain a stat named \"" + name + "\"");
        return 0;
    }

    // Attempts to add modifier to a stat, if that stat does not exist display error
    public void AddModifier(Modifier modifier) {
        if (stats.ContainsKey(modifier.name))
            stats[modifier.name].AddModifier(modifier);
        else
            Debug.LogWarning("Error adding modifier, " + "Player " + player.playerNumber + " does not contain a stat named \"" + modifier.name + "\"");
    }

    // Attempts to remove modifier from a stat, if that stat does not exist display error
    public void RemoveModifier(Modifier modifier) {
        if (stats.ContainsKey(modifier.name))
            stats[modifier.name].RemoveModifier(modifier);
        else
            Debug.LogWarning("Error removing modifier, " + "Player " + player.playerNumber + " does not contain a stat named \"" + modifier.name + "\"");
    }

    // manages the value of a single stat
    private class Stat {
        public float baseValue;
        public Type type;
        private readonly Dictionary<Modifier.ModifierType, List<Modifier>> modifiers;

        private float _value;
        private bool isDirty = true;
        public float Value {
            get { if (isDirty)
                    UpdateValue();
                return _value; }
            private set { _value = value; }
        }

        public Stat(float baseValue, Type type = Type.Numerical) {
            this.baseValue = baseValue;
            this.type = Type.Numerical;
            modifiers = new Dictionary<Modifier.ModifierType, List<Modifier>>();
            modifiers.Add(Modifier.ModifierType.Flat, new List<Modifier>());
            modifiers.Add(Modifier.ModifierType.Increased, new List<Modifier>());
            modifiers.Add(Modifier.ModifierType.More, new List<Modifier>());
            modifiers.Add(Modifier.ModifierType.Bool, new List<Modifier>());
        }

        // Add modifier based on its type;
        public void AddModifier(Modifier modifier) {
            modifiers[modifier.type].Add(modifier);
            isDirty = true;
        }

        // Remove modifier based on its type;
        public bool RemoveModifier(Modifier modifier) {
            if (modifiers[modifier.type].Remove(modifier))
                return isDirty = true;
            return false;
        }

        // Updates the property's value
        private void UpdateValue() {
            switch (type) { // Check the type of property (Numerical or Boolean)
                case Type.Numerical:
                    Value = (baseValue + TotalMods(modifiers[Modifier.ModifierType.Flat])) * (1+TotalMods(modifiers[Modifier.ModifierType.Increased])); // Adds all flat modifiers and multiplies by increased modifiers
                    foreach (Modifier m in modifiers[Modifier.ModifierType.More]) // Multiplies every more modifier separately
                        Value *= 1 + m.value;
                    break;
                case Type.Bool:
                    if (TotalMods(modifiers[Modifier.ModifierType.Bool]) + baseValue > 0) // Check if boolean modifiers add up to true
                        Value = 1;
                    break;
            }
            isDirty = false;
        }

        // Returns sum of all values in a set of modifiers 
        private static float TotalMods (List<Modifier> modifiers) {
            float total = 0;
            foreach (Modifier m in modifiers)
                total += m.value;
            return total;
        }

        public enum Type { Numerical, Bool}; // Whether the property is a boolean value of numerical value
    }

    // A single modifier that can affect a property
    [System.Serializable]
    public class Modifier {
        public string name;
        public float value;
        public ModifierType type;

        public Modifier(string name, float value, ModifierType type = ModifierType.Flat) {
            this.name = name;
            this.value = value;
            this.type = type;
        }

        public enum ModifierType { Flat, Increased, More, Bool}; // How the modifier affects properties
    }
}
