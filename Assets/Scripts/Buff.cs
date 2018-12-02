using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buff {
    public readonly float startTime;
    public readonly float endTime;
    public float timeRemaining {
        get { return endTime - Time.time; }
    }
    public bool isPermenant {
        get { return float.IsPositiveInfinity(endTime); }
    }
    public readonly object Source;

    [SerializeField]
    public List<PlayerStats.Modifier> Modifiers;
    // List of functions to be called every update on a player
    [SerializeField]
    public List<Trigger> Triggers;

    // Create new power-up instance for editing in inspector
    public Buff() {
        Modifiers = new List<PlayerStats.Modifier>();
        Triggers = new List<Trigger>();
    }

    // Contructor used to clone instance of powerup
    private Buff(float duration, object source) {
        Triggers = new List<Trigger>();
        this.Source = source;
        startTime = Time.time;
        endTime = Time.time + duration;
    }

    // Create a deep copy of this powerup instance. Used for when adding a new powerup to a player
    public Buff DeepCopy(float duration, object source) {
        Buff copy = new Buff(duration, source);
        copy.Modifiers = Modifiers;
        foreach (Trigger t in Triggers) {
            copy.Triggers.Add(t.DeepCopy(copy));
        }
        return copy;
    }

    // Contains an activate function and a condition that should activate it
    [System.Serializable]
    public class Trigger {  // Unity does not support Serialization of Derived Classes, so we will have to find another way to make more types of triggers
        public List<PlayerStats.Modifier> modifiers;
        public TriggerCondition condition;
        public float cooldown;
        private float refreshTime = 0;
        public WeaponData triggerWeapon;
        private Weapon weapon;
        private readonly Buff SourceBuff;

        public Trigger() { }

        private Trigger(Buff source) {
            SourceBuff = source;
        }

        public void Enable(Player player) {
            if (SourceBuff.Source.GetType() == typeof(Player)) {
                weapon = triggerWeapon.NewInstance((Player)SourceBuff.Source);
                weapon.EquipWeapon();
            }
            else {
                weapon = triggerWeapon.NewInstance(player);
                weapon.EquipWeapon();
            }
        }

        public void Disable() {
            weapon.UnequipWeapon();
        }

        // Activate weapon if off cooldown
        public void Activate(Vector3 origin, Vector3 direction, PlayerController targetController = null) {
            if (refreshTime <= Time.time) {
                OnActivate(origin, direction, targetController);
                refreshTime = Time.time + cooldown;
            }
        }

        // What happens when trigger is activated
        private void OnActivate(Vector3 origin, Vector3 direction, PlayerController targetController = null) {
            weapon.Activate(origin, direction, targetController);
            weapon.Release();
        }

        // Create a deep copy of this class
        public Trigger DeepCopy(Buff buff) {
            Trigger copy = new Trigger(buff);
            copy.modifiers = modifiers;
            copy.condition = condition;
            copy.cooldown = cooldown;
            copy.triggerWeapon = triggerWeapon;
            return copy;
        }

        // Different types of Trigger Conditions available
        public enum TriggerCondition { Update, Move, Stationary, Jump, Airborn, Land, Attack, Death, Touch, Hurt, StartDodge, EndDodge, Dodging, Hit}
    }
}
