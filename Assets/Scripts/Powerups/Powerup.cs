using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Powerup {
    public PowerupData data;
    public float startTime;
    public float endTime;
    public float timeRemaining {
        get { return endTime - Time.time; }
    }
    public bool isPermenant {
        get { return float.IsPositiveInfinity(endTime); }
    }
    public List<PlayerStats.Modifier> modifiers {
        get { return data.modifiers; }
    }
    internal Player player;

    // List of functions to be called every update on a player
    public List<Trigger> triggers = new List<Trigger>();

    internal Powerup(Player player, bool isPermenant = false) {
        this.player = player;
        startTime = Time.time;
        endTime = isPermenant ? float.PositiveInfinity : Time.time + PowerupData.duration;
    }

    private Powerup(PowerupData data, Player player, bool isPermenant = false) {
        this.data = data;
        this.player = player;
        startTime = Time.time;
        endTime = isPermenant ? float.PositiveInfinity : Time.time + PowerupData.duration;
    }

    //internal void AddUpdate(UnityAction updateMethod) {
    //    onUpdate.AddListener(updateMethod);
    //}

    public Powerup Clone(Player player, bool isPermenant = false) {
        Powerup copy = new Powerup(data, player, isPermenant);
        foreach (Trigger t in triggers) {
            copy.triggers.Add(t.Copy(copy));
        }
        return copy;
    }

    [System.Serializable]
    public class Trigger {
        [System.NonSerialized] // Prevent serialization loop
        private Powerup powerup;
        public GameObject spawnPrefab;
        public float cooldown;
        public List<PlayerStats.Modifier> modifiers;
        public Type type;

        private float refreshTime;

        public Trigger(Powerup powerup) {
            this.powerup = powerup;
            refreshTime = 0;
        }

        public void Activate() {
            if (refreshTime <= Time.time) {
                GameObject.Instantiate(spawnPrefab, powerup.player.transform.position, Quaternion.identity);
                refreshTime = Time.time + cooldown;
            }
        }

        public Trigger Copy(Powerup powerup) {
            Trigger copy = new Trigger(powerup) {
                spawnPrefab = spawnPrefab,
                cooldown = cooldown,
                modifiers = modifiers,
                type = type
            };
            return copy;
        }

        public enum Type { Update, Move, Stationary, Jump, Airborn, Land, Attack, Death, Touch, Hurt, StartDodge, EndDodge, Dodging}
    }
}
