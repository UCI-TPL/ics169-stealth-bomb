using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class Weapon {

    public WeaponData weaponData;
    public Type type;

    protected Player player; // Player that this weapon is attached to
    public float ChargeLevel { get { return Mathf.Clamp(GetChargeLevel(), 0, 1); } }
    protected virtual float GetChargeLevel() {
        return 0;
    }

    protected bool isCharging = false;
    protected int numCharging = 0;// This is how many charging coroutines are active at once, This allows us to limit to one charge
    private bool overrideChargeUpdate = false;

    public Weapon() { }

    // Contructor used to clone instance of weapon
    public Weapon(WeaponData weaponData, Player player, Type type = Type.Instant) {
        this.weaponData = weaponData;
        this.player = player;
        this.type = type;
        overrideChargeUpdate = IsOverride("OnChargingUpdate");
    }

    // Activate weapon. In other words, initiate attack
    public void Activate() {
        if (isCharging)
            Release();
        isCharging = true;
        OnActivate();
        if (overrideChargeUpdate)
            player.controller.StartCoroutine(ChargingUpdate());
    }

    // OnActivate is called once when the weapon is activated
    protected virtual void OnActivate() { }

    // Coroutine to repetedly call OnChargingUpdate while weapon is charging
    private IEnumerator ChargingUpdate() {
        ++numCharging;
        while (isCharging && numCharging == 1) {
            OnChargingUpdate();
            yield return null;
        }
        --numCharging;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected virtual void OnChargingUpdate() { }

    // Release attack or ending a charge
    public void Release() {
        if (isCharging) {
            OnRelease();
            isCharging = false;
        }
    }

    // OnRelease is called once when the weapon is released
    protected virtual void OnRelease() { }

    // Stop all processes in a weapon before removing
    public void RemoveWeapon() {
        Release();
    }

    // Create a deep copy of this powerup instance. Used for when adding a new powerup to a player
    public abstract Weapon DeepCopy(WeaponData weaponData, Player player);

    // Classify weapons as charged or instant, Charged attacks cannot be used as triggers
    public enum Type {
        Instant, Charge, Move
    }

    // This method was taken from https://stackoverflow.com/questions/7233905/how-to-detect-if-virtual-method-is-overridden-in-c-sharp/7234217#7234217
    private bool IsOverride(string methodName) {
        return !(GetType().GetMember(methodName,
               BindingFlags.NonPublic
             | BindingFlags.Instance
             | BindingFlags.DeclaredOnly).Length == 0);
    }
}
