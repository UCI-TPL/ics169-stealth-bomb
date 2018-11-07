using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player {
    
    public PlayerStats stats { get; private set; }
    public PlayerController controller { get; private set; }
    public PlayerData playerData { get; private set; }

    public int playerNumber;
    
    public float health { get; private set; }
    public float experiance { get; private set; }
    public int rank { get { return Mathf.FloorToInt(experiance) + 1; } }

    private List<Powerup> permPowerups = new List<Powerup>();
    private List<Powerup> powerups = new List<Powerup>();
    // The Last player this player was hurt by. This is for attributing kills
    private Player lastHurtBy;

    // Events
    private UnityEvent onUpdate = new UnityEvent();
    private UnityEvent onMove = new UnityEvent();
    public delegate void onHurtDel(Player damageDealer, Player reciever, float percentDealt);
    public event onHurtDel onHurt;
    public UnityEvent onHeal = new UnityEvent();
    public delegate void onDeathDel(Player killer, Player killed);
    public event onDeathDel onDeath;
    public delegate void powerupDel(Powerup powerup);
    public event powerupDel onAddPowerUp;

    // Currently equiped weapon
    public Weapon weapon;

    private Vector3 prevPosition;

    public Player(int playerNumber, PlayerData playerData) {
        this.playerNumber = playerNumber;
        this.playerData = playerData;
        stats = new PlayerStats(this); // Creates a stats profile for the player
        ResetHealth();
        ResetWeapon();
        experiance = 0;
    }

    public void SetController(PlayerController controller) {
        this.controller = controller;
        controller.player = this;
    }

    public void ResetForRound() {
        ResetHealth();
        for (int i = powerups.Count - 1; i >= 0; --i) {
            RemovePowerup(powerups[i]);
        }
    }

    public void AddExperiance(float amount) {
        experiance += amount;
    }

    public void DisablePlayer(float duration) { //this exists for GameController to access PlayerController
        controller.DisableMovement(duration);
        controller.DisableAttack(duration);
    }

    public void ResetWeapon() {
        weapon = playerData.defaultWeapon.NewInstance(this);
    }

    public void ResetHealth() {
        health = stats.maxHealth;
    }

    public void Heal(float amount) {
        health = Mathf.Min(health + amount, stats.maxHealth);
        onHeal.Invoke();
    }

    /// <summary>Deals damage to this player and returns the percentage of damage dealt</summary>
    public float Hurt(Player damageDealer, float damage) {
        float damageDealt = Mathf.Min(damage, health);
        health -= damageDealt;
        float percent = damageDealt / stats.maxHealth; // Percentage of max health dealt
        lastHurtBy = damageDealer; // Remember last player that dealt damage, this is used to attribute kills
        onHurt.Invoke(damageDealer, this, percent);
        return percent;
    }

    public void Kill() {
        health = 0;
    }
    
    private void CheckDeath() {
        if (health <= 0) {
            controller.input.controllers[playerNumber].Vibrate(1.0f, 1f, InputManager.Controller.VibrationMode.Diminish);
            onDeath(lastHurtBy, this);
            GameObject.Destroy(controller.gameObject);
        }
    }

    public void InGameUpdate() {
        List<Powerup> deleteList = new List<Powerup>();
        foreach (Powerup p in powerups) {
            if (p.endTime <= Time.time)
                deleteList.Add(p); // Remove power-up if expired
        }
        foreach (Powerup p in deleteList)
            RemovePowerup(p);

        onUpdate.Invoke();
        if (controller.isMoving && controller.isGrounded)
            onMove.Invoke();

        CheckDeath();
    }

    // Determine the Type of item and handle accordingly
    public void AddItem(ItemData data) {
        switch(data.type) {
            case ItemData.Type.Item:
                data.Use(this);
                break;
            case ItemData.Type.Powerup:
                AddPowerup((PowerupData)data);
                break;
            case ItemData.Type.Weapon:
                weapon.RemoveWeapon();
                ChangeWeapon((WeaponData)data);
                break;
        }
    }

    // Create a new instance of a power-up, save it to list of power-ups and add its modifiers to stats
    public void AddPowerup(PowerupData powerupData) {
        Powerup powerup;
        if (permPowerups.Count < rank) {
            powerup = powerupData.NewInstance(this, true); // Initialize power-up
            permPowerups.Add(powerup); // Save to list of powerups
        } else {
            powerup = powerupData.NewInstance(this); // Initialize power-up
            powerups.Add(powerup); // Save to list of powerups
        }
        onAddPowerUp(powerup);
        foreach (PlayerStats.Modifier m in powerup.modifiers) // Add power-up's modifiers to stats
            stats.AddModifier(m);
        foreach (Powerup.Trigger t in powerup.triggers) { // Add all the powerup's triggers to the respective event calls
            switch (t.type) {
                case Powerup.Trigger.Type.Update:
                    onUpdate.AddListener(t.Activate);
                    break;
                case Powerup.Trigger.Type.Move:
                    onMove.AddListener(t.Activate);
                    break;
            }
        }
    }

    // Remove each modifier granted by the power-up and remove the power-up from list of power-ups
    public void RemovePowerup(Powerup powerup) {
        foreach (PlayerStats.Modifier m in powerup.modifiers) // Remove all modifiers granted by this powerup
            stats.RemoveModifier(m);
        foreach (Powerup.Trigger t in powerup.triggers) {// Remove all the powerup's triggers from the respective event calls
            switch (t.type) {
                case Powerup.Trigger.Type.Update:
                    onUpdate.RemoveListener(t.Activate);
                    break;
                case Powerup.Trigger.Type.Move:
                    onMove.RemoveListener(t.Activate);
                    break;
            }
        }
        powerups.Remove(powerup); // Remove powerup from list of powerups
    }

    public void ChangeWeapon(WeaponData data) {
        weapon = data.NewInstance(this);
    }
}
