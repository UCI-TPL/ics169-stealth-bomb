using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buff {
    public float duration;
    public readonly float startTime;
    public readonly float endTime;
    public float timeRemaining {
        get { return endTime - Time.time; }
    }
    public bool isPermenant {
        get { return float.IsPositiveInfinity(endTime); }
    }
    [SerializeField]
    public List<PlayerStats.Modifier> modifiers;
    internal Player player; // Player that this powerup is attached to

    // List of functions to be called every update on a player
    public List<Trigger> triggers = new List<Trigger>();

    // Create new power-up instance for editing in inspector
    public Buff() { }

    // Contructor used to clone instance of powerup
    private Buff(Player player, bool isPermenant = false) {
        this.player = player;
        startTime = Time.time;
        endTime = isPermenant ? float.PositiveInfinity : Time.time + duration;
    }

    // Create a deep copy of this powerup instance. Used for when adding a new powerup to a player
    public Buff Clone(Player player, bool isPermenant = false) {
        Buff copy = new Buff(player, isPermenant);
        foreach (Trigger t in triggers) {
            copy.triggers.Add(t.Copy(copy));
        }
        return copy;
    }

    // Contains a weapon that activates when a condition is triggered
    [System.Serializable]
    public class Trigger {
        [System.NonSerialized] // Prevent serialization loop
        private Buff buff;
        public WeaponData triggerWeapon;
        private Weapon weapon;
        public float cooldown;
        public List<PlayerStats.Modifier> modifiers;
        public Type type;

        private float refreshTime;

        public Trigger(WeaponData triggerWeapon, Buff buff) {
            this.buff = buff;
            this.triggerWeapon = triggerWeapon;
            weapon = triggerWeapon.NewInstance(buff.player);
            refreshTime = 0;
        }

        // Activate weapon if off cooldown
        public void Activate() {
            if (refreshTime <= Time.time) {
                weapon.Activate();
                refreshTime = Time.time + cooldown;
            }
        }

        // Create a deep copy of this class
        public Trigger Copy(Buff buff) {
            Trigger copy = new Trigger(triggerWeapon, buff) {
                cooldown = cooldown,
                modifiers = modifiers,
                type = type
            };
            return copy;
        }

        // Different types of Triggers available
        public enum Type { Update, Move, Stationary, Jump, Airborn, Land, Attack, Death, Touch, Hurt, StartDodge, EndDodge, Dodging }
    }
}
