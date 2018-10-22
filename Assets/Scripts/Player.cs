using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Player requires the GameObject to have a PlayerController and PlayerStats component
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStats))]
public class Player : MonoBehaviour {

    private PlayerStats _stats;
    public PlayerStats stats {
        get { return _stats; }
        private set { _stats = value; }
    }
    private PlayerController _controller;
    public PlayerController controller {
        get { return _controller; }
        private set { _controller = value; }
    }

    private float _health;
    public float health {
        get { return _health; }
        private set { _health = value; }
    }

    private Rigidbody rb;

    private Queue<PowerupContainer> tempPowerups = new Queue<PowerupContainer>();

    private Vector3 prevPosition;
    public bool isMoving {
        get { return rb.velocity.magnitude > 0.1f; }
    }

    private void Awake() {
        stats = GetComponent<PlayerStats>();
        if (stats == null) // Check to ensure PlayerStats component is present, since PlayerStats is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerStats Component");
        controller = GetComponent<PlayerController>();
        if (controller == null) // Check to ensure PlayerController component is present, since PlayerController is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerController Component");
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        ResetHealth();
    }

    public void ResetHealth() {
        health = stats.maxHealth;
    }

    public void HurtPlayer(int damage) {
        health -= damage;
    }

    private void CheckDeath() {
        if (health <= 0) {
            //Debug.Log("D E A T H ");
            Destroy(gameObject);
        }
    }

    void Update() {
        while (tempPowerups.Count > 0 && tempPowerups.Peek().endTime <= Time.time) // Check oldest power-up's duration for expiration
             RemovePowerup(tempPowerups.Dequeue().powerup); // Remove power-up from queue and remove its effects

        foreach (PowerupContainer p in tempPowerups) {
            p.onUpdate.Invoke();
        }
        CheckDeath();
    }

    // Add the power-up to queue and add its modifiers to stats
    public void AddPowerup(Powerup powerup) {
        powerup.OnAdd(this); // Initialize power-up
        tempPowerups.Enqueue(new PowerupContainer(this, powerup));
        foreach (PlayerStats.Modifier m in powerup.modifiers)
            stats.AddModifier(m);
    }

    // Remove each modifier granted by the power-up
    public void RemovePowerup(Powerup powerup) {
        foreach (PlayerStats.Modifier m in powerup.modifiers)
            stats.RemoveModifier(m);
    }

    // Holds a powerup and its duration
    private class PowerupContainer {
        public Powerup powerup;
        public float startTime;
        public float endTime;
        public float timeRemaining {
            get { return endTime - Time.time; }
        }
        public bool isPermenant {
            get { return float.IsPositiveInfinity(endTime); }
        }
        private Player player;

        // List of functions to be called every update on a player
        public UnityEvent onUpdate = new UnityEvent();

        public PowerupContainer(Player player, Powerup powerup, bool isPermenant = false) {
            this.player = player;
            this.powerup = powerup;
            startTime = Time.time;
            endTime = isPermenant ? float.PositiveInfinity : Time.time + Powerup.duration;
            onUpdate.AddListener(delegate { powerup.OnUpdate(player); });
        }
    }
}
