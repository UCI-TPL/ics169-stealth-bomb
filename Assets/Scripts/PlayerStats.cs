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
        AddStat("move_speed", 10);
        AddStat("jump_force", 5);
        AddStat("max_health", 100);
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
    }

    public float moveSpeed {
        get { return GetStat("move_speed"); }
    }
    public float jumpForce {
        get { return GetStat("jump_force"); }
    }
    public float maxHealth {
        get { return GetStat("max_health"); }
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
            Debug.LogError(gameObject.name + " does not contain a stat named \"" + modifier.name + "\"");
    }

    // Attempts to remove modifier from a stat, if that stat does not exist display error
    public void RemoveModifier(Modifier modifier) {
        if (stats.ContainsKey(modifier.name))
            stats[modifier.name].RemoveModifier(modifier);
        else
            Debug.LogError(gameObject.name + " does not contain a stat named \"" + modifier.name + "\"");
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
        private HashSet<Modifier> flatModifiers;
        private HashSet<Modifier> incModifiers;
        private HashSet<Modifier> moreModifiers;
        private HashSet<Modifier> boolModifiers;

        public Property(string name, float baseValue, Type type = Type.Numerical) {
            this.name = name;
            this.baseValue = baseValue;
            this.type = Type.Numerical;
            flatModifiers = new HashSet<Modifier>();
            incModifiers = new HashSet<Modifier>();
            moreModifiers = new HashSet<Modifier>();
            boolModifiers = new HashSet<Modifier>();
            UpdateValue();
        }

        // Add modifier based on its type;
        public void AddModifier(Modifier modifier) {
            switch (modifier.type) {
                case Modifier.Type.Flat:
                    flatModifiers.Add(modifier);
                    break;
                case Modifier.Type.Increased:
                    incModifiers.Add(modifier);
                    break;
                case Modifier.Type.More:
                    moreModifiers.Add(modifier);
                    break;
                case Modifier.Type.Bool:
                    boolModifiers.Add(modifier);
                    break;
            }
            UpdateValue();
        }

        // Remove modifier based on its type;
        public void RemoveModifier(Modifier modifier) {
            switch (modifier.type) {
                case Modifier.Type.Flat:
                    flatModifiers.Remove(modifier);
                    break;
                case Modifier.Type.Increased:
                    incModifiers.Remove(modifier);
                    break;
                case Modifier.Type.More:
                    moreModifiers.Remove(modifier);
                    break;
                case Modifier.Type.Bool:
                    boolModifiers.Remove(modifier);
                    break;
            }
            UpdateValue();
        }

        // Updates the property's value
        private void UpdateValue() {
            switch (type) { // Check the type of property (Numerical or Boolean)
                case Type.Numerical:
                    value = (baseValue + TotalMods(flatModifiers)) * (1+TotalMods(incModifiers)); // Adds all flat modifiers and multiplies by increased modifiers
                    foreach (Modifier m in moreModifiers) // Multiplies every more modifier separately
                        value *= 1+m.value;
                    break;
                case Type.Bool:
                    if (TotalMods(boolModifiers) + baseValue > 0) // Check if boolean modifiers add up to true
                        value = 1;
                    break;
            }
        }

        // Returns sum of all values in a set of modifiers 
        private float TotalMods (HashSet<Modifier> modifiers) {
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
        public Type type;

        public Modifier(string name, float value, Type type = Type.Flat) {
            this.name = name;
            this.value = value;
            this.type = type;
        }

        public enum Type { Flat, Increased, More, Bool}; // How the modifier affects properties
    }
}
