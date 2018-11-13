using System;
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

    private List<Buff> permPowerups = new List<Buff>();
    private List<Buff> buffs = new List<Buff>();
    // The Last player this player was hurt by. This is for attributing kills
    private Player lastHurtBy;

    // Events
    public delegate void playerEventDel(Player player);
    private event playerEventDel OnUpdate;
    private event playerEventDel OnMove;
    public delegate void onHurtDel(Player damageDealer, Player reciever, float percentDealt);
    public event onHurtDel OnHurt;
    public UnityEvent onHeal = new UnityEvent();
    public delegate void onDeathDel(Player killer, Player killed);
    public event onDeathDel OnDeath;
    public delegate void powerupDel(PowerupData powerupData, Buff buff);
    public event powerupDel OnAddPowerUp;

    // Currently equiped weapon
    public Weapon weapon;

    public Weapon specialMove; //give this to the controlelr

    private Vector3 prevPosition;

    public Player(int playerNumber, PlayerData playerData) {
        this.playerNumber = playerNumber;
        this.playerData = playerData;
        stats = new PlayerStats(this); // Creates a stats profile for the player
        ResetHealth();
        ResetWeapon();
        ResetSpecialMove();
        experiance = 0;
    }

    public void SetController(PlayerController controller) {
        this.controller = controller;
        controller.player = this;
    }

    public void ResetForRound() {
        ResetHealth();
        for (int i = buffs.Count - 1; i >= 0; --i)
            RemoveBuff(buffs[i]);
        lastHurtBy = null;
    }

    public void AddExperiance(float amount) {
        experiance += amount;
    }

    public void DisablePlayer(float duration) { //this exists for GameController to access PlayerController
        controller.DisableMovement(duration);
        controller.DisableAttack(duration);
    }

    public void ResetSpecialMove()    { 
        specialMove = playerData.defaultSpecialMove.NewInstance(this);
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
        OnHurt.Invoke(damageDealer, this, percent);
        return percent;
    }

    public void Kill() {
        health = 0;
    }
    
    private void CheckDeath() {
        if (health <= 0) {
            controller.input.controllers[playerNumber].Vibrate(1.0f, 1f, InputManager.Controller.VibrationMode.Diminish);
            OnDeath(lastHurtBy, this);
            GameObject.Destroy(controller.gameObject);
        }
    }

    public void InGameUpdate() {
        while (buffs.Count > 0 && buffs[0].endTime <= Time.time)
            RemoveBuff(buffs[0]);

        if (OnUpdate != null)
            OnUpdate(this);
        if (controller.isMoving && controller.isGrounded)
            if (OnMove != null)
                OnMove(this);

        CheckDeath();
    }

    // Determine the Type of item and handle accordingly
    public void AddItem(ItemData data) {
        data.Use(this);
    }

    // Create a new instance of a power-up, save it to list of power-ups and add its modifiers to stats
    public void AddBuff(Buff buff) {
        buffs.Add(buff); // Save to list of buffs
        buffs.Sort(CompareLowerBuffDuration); // Always sort 
        foreach (PlayerStats.Modifier m in buff.Modifiers) // Add power-up's modifiers to stats
            stats.AddModifier(m);
        foreach (Buff.Trigger t in buff.Triggers) { // Add all the powerup's triggers to the respective event calls
            switch (t.condition) {
                case Buff.Trigger.TriggerCondition.Update:
                    OnUpdate += t.Activate;
                    break;
                case Buff.Trigger.TriggerCondition.Move:
                    OnMove += t.Activate;
                    break;
            }
        }
        if (buff.Source.GetType() == typeof(PowerupData))
            OnAddPowerUp((PowerupData)buff.Source, buff);
    }

    private static int CompareLowerBuffDuration(Buff L, Buff R) {
        return L.timeRemaining <= R.timeRemaining ? -1 : 1;
    }
    
    // Remove each modifier granted by the power-up and remove the power-up from list of power-ups
    public void RemoveBuff(Buff buff) {
        foreach (PlayerStats.Modifier m in buff.Modifiers) // Remove all modifiers granted by this powerup
            stats.RemoveModifier(m);
        foreach (Buff.Trigger t in buff.Triggers) {// Remove all the powerup's triggers from the respective event calls
            switch (t.condition) {
                case Buff.Trigger.TriggerCondition.Update:
                    OnUpdate -= t.Activate;
                    break;
                case Buff.Trigger.TriggerCondition.Move:
                    OnMove -= t.Activate;
                    break;
            }
        }
        buffs.Remove(buff); // Remove powerup from list of powerups
    }

    public void ChangeWeapon(WeaponData data) {
        weapon.RemoveWeapon();
        weapon = data.NewInstance(this);
    }
}
