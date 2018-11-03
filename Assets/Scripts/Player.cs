﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Player requires the GameObject to have a PlayerController and PlayerStats component
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStats))]
public class Player : MonoBehaviour {
    
    public PlayerStats stats { get; private set; }
    public PlayerController controller { get; private set; }

    public int playerNumber;
    
    public float health { get; private set; }

    public Color[] Colors = new Color[4]; //The player's color is based on thier player number
    public Color playerColor { get { return Colors[playerNumber]; } set { Colors[playerNumber] = value; } }//used for the bullettrail

    private Rigidbody rb;

    public Renderer rend;

    private List<Powerup> powerups = new List<Powerup>();
    
    private UnityEvent onUpdate = new UnityEvent();
    private UnityEvent onMove = new UnityEvent();

    // Currently equiped weapon
    public Weapon weapon;
    public WeaponData DefaultWeapon;

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
        rend = rend == null ? GetComponent<Renderer>() : rend;
        //if (Colors.Length == 0)
        //    Colors[playerNumber] = Color.gray; //make sure the default color is gray and not black
        rend.material.color = playerColor; //setting the player color based on playeNum 
        
    }

    private void Start() {
        ResetHealth();
        ResetWeapon();
    }

    public void ResetWeapon() {
        weapon = DefaultWeapon.NewInstance(this);
    }

    public void ResetHealth() {
        health = stats.maxHealth;
    }

    public void Heal(float amount) {
        health = Mathf.Min(amount, stats.maxHealth);
    }

    public void HurtPlayer(float damage) {
        health -= damage;
        controller.input.controllers[playerNumber].Vibrate(1.0f, 0.1f);
    }

    private void CheckDeath() {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    void Update() {
        List<Powerup> deleteList = new List<Powerup>();
        foreach (Powerup p in powerups) {
            if (p.endTime <= Time.time)
                deleteList.Add(p); // Remove power-up if expired
        }
        foreach (Powerup p in deleteList)
            RemovePowerup(p);

        onUpdate.Invoke();
        if (isMoving && controller.isGrounded)
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
        Powerup powerup = powerupData.NewInstance(this); // Initialize power-up
        powerups.Add(powerup); // Save to list of powerups
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
