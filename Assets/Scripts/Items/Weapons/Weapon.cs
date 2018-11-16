using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class Weapon {

    public WeaponData weaponData;
    public Type type;

    protected Player player; // Player that this weapon is attached to
    public float ChargeLevel { get { return isCharging ? Mathf.Clamp(GetChargeLevel(), 0, 1) : 0; } }
    protected virtual float GetChargeLevel() {
        return 0;
    }

    private float cooldownExpirationTime = 0;
    public bool OffCooldown { get { return cooldownExpirationTime <= Time.time; } }
    private bool attackQueued = false;

    protected bool isCharging = false;
    protected int numCharging = 0; // This is how many charging coroutines are active at once, This allows us to ensure only one charge at a time
    private bool overrideChargeUpdate = false;

    public Weapon() { }

    // Contructor used to clone instance of weapon
    public Weapon(WeaponData weaponData, Player player, Type type = Type.Instant) {
        this.weaponData = weaponData;
        this.player = player;
        this.type = type;
        overrideChargeUpdate = IsOverride("OnChargingUpdate");
    }

    // Called when player equips weapon either by swaping weapons or first starting game
    public void EquipWeapon(PlayerController controller) {
        Start();
        controller.OnDestroyEvent.AddListener(OnDestroyEvent); 
    }

    // Start is called once when the weapon is first loaded in game use this to ensure PlayerController is active
    protected virtual void Start() { }

    // OnDestroy is called when the player controller is destroyed;
    private void OnDestroyEvent() {
        attackQueued = false; // Make sure to remove queued attack
        isCharging = false; // Make sure to reset charging
        End();
    }

    // End is called once when the weapon is removed from game
    protected virtual void End() { }

    // Activate weapon. In other words, initiate attack
    public void Activate() {
        if (OffCooldown) { // Activate if off cooldown
            cooldownExpirationTime = Time.time + weaponData.cooldown; // reset cooldown
            attackQueued = false; // removed queued attack
            if (isCharging) // If the weapon is already charging, release the charge and reactivate
                Release();
            isCharging = true;
            OnActivate();
            if (overrideChargeUpdate) // Only start OnCharginUpdate coroutine if its been overriden in derived class, This is for slight optimization
                player.controller.StartCoroutine(ChargingUpdate(numCharging));
        } else if (!attackQueued) { // Queue another attack if attack is on cooldown
            attackQueued = true;
            player.controller.StartCoroutine(QueueingAttack());
        }
    }

    private IEnumerator QueueingAttack() {
        while (attackQueued) {
            Activate();
            yield return null;
        }
    }

    // OnActivate is called once when the weapon is activated
    protected virtual void OnActivate() { }

    // Coroutine to repetedly call OnChargingUpdate while weapon is charging
    private IEnumerator ChargingUpdate(int numCharging) {
        ++this.numCharging; // Increment current charging number
        ++numCharging;
        while (isCharging && this.numCharging == numCharging) { // Stop if current charging number has changed, This prevents there every being more than one charging coroutine active
            OnChargingUpdate();
            yield return null;
        }
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected virtual void OnChargingUpdate() { }

    // Release attack or ending a charge
    public void Release() {
        attackQueued = false; // Unqueue attack if attack button is released
        if (isCharging) {
            OnRelease();
            isCharging = false;
        }
    }

    // OnRelease is called once when the weapon is released
    protected virtual void OnRelease() { }

    // Stop all processes in a weapon before removing
    public void RemoveWeapon(PlayerController controller) {
        Release();
        controller.OnDestroyEvent.RemoveListener(OnDestroyEvent);
        End();
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
