using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    private Player _player;
    public Player player {
        get { return _player; }
        private set { _player = value; }
    }

    private Dictionary<string, Property> stats = new Dictionary<string, Property>();

    private void Awake() {
        AddStat("move_speed", 5); //blocks per second
        AddStat("air_speed", 50);  //percent of movespeed
        AddStat("jump_force", 1); //blocks per jump (1,2,3)
        AddStat("max_health", 100);
        AddStat("dodge_time", 0.2f);
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
    }

    public float moveSpeed {
        get { return GetStat("move_speed") * 1.75f; }
    }
    public float jumpForce {
        get { return 9 + (GetStat("jump_force") - 1) * 3; } //1 return 9, 2 returns 12, 3 returns 15
    }
    public float maxHealth {
        get { return GetStat("max_health"); }
    }
    public float airSpeed    {
        get { return (GetStat("air_speed")/100) * moveSpeed;  }
    }
    public float dodgeTime    {
        get { return GetStat("dodge_time"); }
    }


    // Add a new stat
    private void AddStat(string name, float value) {
        stats.Add(name, new Property(name, value));
    }

    // Attempts to grab a stat, if that stat does not exist display error
    public float GetStat(string name) {
        if (stats.ContainsKey(name))
            return stats[name].value;
        Debug.LogError(gameObject.name + " does not contain a stat named \"" + name + "\"");
        return 0;
    }

    // Attempts to add modifier to a stat, if that stat does not exist display error
    public void AddModifier(Modifier modifier) {
        if (stats.ContainsKey(modifier.name))
            stats[modifier.name].AddModifier(modifier);
        else
            Debug.LogError("Error adding modifier, " + gameObject.name + " does not contain a stat named \"" + modifier.name + "\"");
    }

    // Attempts to remove modifier from a stat, if that stat does not exist display error
    public void RemoveModifier(Modifier modifier) {
        if (stats.ContainsKey(modifier.name))
            stats[modifier.name].RemoveModifier(modifier);
        else
            Debug.LogError("Error removing modifier, " + gameObject.name + " does not contain a stat named \"" + modifier.name + "\"");
    }

    // manages the value of a single stat
    private class Property {
        private string name;
        public float baseValue;
        public Type type;
        private float _value;
        public float value {
            get { return _value; }
            private set { _value = value; }
        }
        private Dictionary<Modifier.Type, Dictionary<Modifier, int>> modifiers;

        public Property(string name, float baseValue, Type type = Type.Numerical) {
            this.name = name;
            this.baseValue = baseValue;
            this.type = Type.Numerical;
            modifiers = new Dictionary<Modifier.Type, Dictionary<Modifier, int>>();
            modifiers.Add(Modifier.Type.Flat, new Dictionary<Modifier, int>());
            modifiers.Add(Modifier.Type.Increased, new Dictionary<Modifier, int>());
            modifiers.Add(Modifier.Type.More, new Dictionary<Modifier, int>());
            modifiers.Add(Modifier.Type.Bool, new Dictionary<Modifier, int>());
            UpdateValue();
        }

        // Add modifier based on its type;
        public void AddModifier(Modifier modifier) {
            IncMod(modifiers[modifier.type], modifier);
            UpdateValue();
        }

        // Remove modifier based on its type;
        public void RemoveModifier(Modifier modifier) {
            DecMod(modifiers[modifier.type], modifier);
            UpdateValue();
        }

        // Adds the modifier to the dictionary incrementing the counter if it already exist
        private void IncMod(Dictionary<Modifier, int> dict, Modifier mod) {
            if (dict.ContainsKey(mod))
                dict[mod] += 1;
            else
                dict.Add(mod, 1);
        }

        // Removes the modifier from the dictionary decresing the counter if there are more than 1
        private void DecMod(Dictionary<Modifier, int> dict, Modifier mod) {
            if (dict[mod] <= 1)
                dict.Remove(mod);
            else
                dict[mod] -= 1;
        }

        // Updates the property's value
        private void UpdateValue() {
            switch (type) { // Check the type of property (Numerical or Boolean)
                case Type.Numerical:
                    value = (baseValue + TotalMods(modifiers[Modifier.Type.Flat])) * (1+TotalMods(modifiers[Modifier.Type.Increased])); // Adds all flat modifiers and multiplies by increased modifiers
                    foreach (KeyValuePair<Modifier, int> m in modifiers[Modifier.Type.More]) // Multiplies every more modifier separately
                        value *= Mathf.Pow(1+m.Key.value, m.Value);
                    break;
                case Type.Bool:
                    if (TotalMods(modifiers[Modifier.Type.Bool]) + baseValue > 0) // Check if boolean modifiers add up to true
                        value = 1;
                    break;
            }
        }

        // Returns sum of all values in a set of modifiers 
        private float TotalMods (Dictionary<Modifier,int> modifiers) {
            float total = 0;
            foreach (KeyValuePair<Modifier, int> m in modifiers)
                total += m.Key.value * m.Value;
            return total;
        }

        public enum Type { Numerical, Bool}; // Whether the property is a boolean value of numerical value
    }

    // A single modifier that can affect a property
    [System.Serializable]
    public class Modifier {
        public string name;
        public float value;
        public Type type;

        public Modifier(string name, float value, Type type = Type.Flat) {
            this.name = name;
            this.value = value;
            this.type = type;
        }

        public enum Type { Flat, Increased, More, Bool}; // How the modifier affects properties
    }
}
