using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : UnityEvent<Vector3, Vector3, PlayerController> { }

public class Player : IHurtable {
    
    public PlayerStats stats { get; private set; }
    public PlayerController controller { get; private set; }
    public PlayerData playerData { get; private set; }

    public int playerNumber;
    public int inputControllerNumber;

    public int colorIndex { get; private set; }
    public Color Color { get { return GameManager.instance.DefaultPlayerData.Colors[colorIndex]; } } // Player's color 

    public float health { get; private set; }
    public int experiance { get; private set; }
    public int rank { get { return experiance / GameManager.instance.ExperianceSettings.PointsPerLevel; } }

    private List<Buff> permPowerups = new List<Buff>();
    private List<Buff> buffs = new List<Buff>();
    // The Last player this player was hurt by. This is for attributing kills
    private Player lastHurtBy;

    // Events
    public TriggerAction OnUpdate = new TriggerAction();
    public TriggerAction OnMove = new TriggerAction();
    public TriggerAction OnHit = new TriggerAction();
    public delegate void onHurtDel(Player damageDealer, Player reciever, float percentDealt);
    public event onHurtDel OnHurt;
    public UnityEvent onHeal = new UnityEvent();
    public delegate void onDeathDel(Player killer, Player killed);
    public event onDeathDel OnDeath;
    public delegate void powerupDel(PowerupData powerupData, Buff buff);
    public event powerupDel OnAddPowerUp;

    public bool ghost = false;
    public bool alive = true;

    // Currently stored weapon
    public WeaponData weaponData;

    public Weapon specialMove; //give this to the controlelr

    public bool Invincible = false;

    public Player(int playerNumber, int xboxControllerNumber, PlayerData playerData) {
        this.playerNumber = playerNumber;
        this.inputControllerNumber = xboxControllerNumber;
        this.playerData = playerData;
        this.colorIndex = GameManager.instance.AssignPlayerColor(); //this decides what color the player will be
        stats = new PlayerStats(this); // Creates a stats profile for the player
        ResetHealth();
        ResetWeapon();
        ResetSpecialMove();
        experiance = 0;
    }

    public void SetGhost(PlayerController controller)
    {
        Color prevColor = this.controller.playerColor;
        this.controller = controller;
        controller.player = this;
        controller.playerColor = prevColor; //all the cursors have the playercolor to help tell them apart
        //controller.EquipWeapon(weaponData.NewInstance(this));
        ghost = true;
    }

    public void SetController(PlayerController controller) {
        this.controller = controller;
        controller.player = this;
        controller.EquipWeapon(weaponData.NewInstance(this));
        ghost = false; //this variable is true when the ghost spawns
        alive = true; //this variable is false when the player dies (which is before the ghost is spawned)
       
    }

    public void ResetForRound() {
        ResetHealth();
        for (int i = buffs.Count - 1; i >= 0; --i)
            RemoveBuff(buffs[i]);
        lastHurtBy = null;
    }

    public float AddExperiance(int amount) {
        experiance += amount;
        return amount;
    }

    public void DisablePlayer(float duration) { //this exists for GameController to access PlayerController
        controller.DisableMovement(duration);
        controller.DisableAttack(duration);
    }

    public void ResetSpecialMove()    { 
        specialMove = playerData.defaultSpecialMove.NewInstance(this);
    }

    public void ResetWeapon() {
        ChangeWeapon(playerData.defaultWeapon);
    }

    public void ResetHealth() {
        health = stats.maxHealth;
    }

    public void Heal(float amount) {
        health = Mathf.Min(health + amount, stats.maxHealth);
        onHeal.Invoke();
    }

    public void EnableInvincibility(float duration)
    {
        controller.StartCoroutine(EnableInvincibilityTimer(duration));
    }

    private IEnumerator EnableInvincibilityTimer(float duration)
    {
        Invincible = true;
        yield return new WaitForSeconds(duration);
        Invincible = false;
    }

    /// <summary>Deals damage to this player and returns the percentage of damage dealt</summary>
    public float Hurt(Player damageDealer, float damage) {
        if (Invincible || ghost) //no damage is done if invincable
            return 0.0f;
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
            
            if (!ghost && alive)
            {
                controller.input.controllers[playerNumber].Vibrate(1.0f, 1f, InputManager.Controller.VibrationMode.Diminish);
                alive = false; //to make sure that EXP is only awarded once 
                OnDeath(lastHurtBy, this);
            }
            controller.Destroy();
            
        }
    }

    public void InGameUpdate() {
        while (buffs.Count > 0 && buffs[buffs.Count-1].endTime <= Time.time) // Check end of buffs list for expired buffs
            RemoveBuff(buffs[buffs.Count - 1]);

        //if (OnUpdate != null)
        //    OnUpdate.Invoke(this);
        //if (controller.IsMoving && controller.IsGrounded)
        //    if (OnMove != null)
        //        OnMove.Invoke(this);

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

        buff.Equip(this);

        if (buff.Source.GetType() == typeof(PowerupData))
            if (OnAddPowerUp != null)
                OnAddPowerUp((PowerupData)buff.Source, buff);
    }

    private static int CompareLowerBuffDuration(Buff L, Buff R) {
        return L.timeRemaining >= R.timeRemaining ? -1 : 1; // Buffs expiring sooner go at the end
    }
    
    // Remove each modifier granted by the power-up and remove the power-up from list of power-ups
    public void RemoveBuff(Buff buff) {
        if (buffs.Remove(buff)) { // Remove powerup from list of powerups
            buff.Unequip(this);
        }
    }

    public void ChangeWeapon(WeaponData data) {
        weaponData = data;
        if (controller != null)
            controller.EquipWeapon(weaponData.NewInstance(this));
    }
}
