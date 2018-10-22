using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Queue<TempPowerup> tempPowerups = new Queue<TempPowerup>();

    private void Awake() {
        stats = GetComponent<PlayerStats>();
        if (stats == null) // Check to ensure PlayerStats component is present, since PlayerStats is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerStats Component");
        controller = GetComponent<PlayerController>();
        if (controller == null) // Check to ensure PlayerController component is present, since PlayerController is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerController Component");
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
        print(stats.moveSpeed);
        while (tempPowerups.Count > 0 && tempPowerups.Peek().endTime <= Time.time)
             RemovePowerup(tempPowerups.Dequeue().powerup);
        CheckDeath();
    }

    public void AddPowerup(Powerup powerup) {
        tempPowerups.Enqueue(new TempPowerup(powerup));
        foreach (PlayerStats.Modifier m in powerup.modifiers)
            stats.AddModifier(m);
    }

    public void RemovePowerup(Powerup powerup) {
        foreach (PlayerStats.Modifier m in powerup.modifiers)
            stats.RemoveModifier(m);
    }

    // Holds a powerup and its duration
    private struct TempPowerup {
        public Powerup powerup;
        public float endTime;

        public TempPowerup(Powerup powerup) {
            this.powerup = powerup;
            this.endTime = Time.time + Powerup.duration;
        }
    }
}
