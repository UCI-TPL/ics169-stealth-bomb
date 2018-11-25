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

    private bool AutoAttack { get { return weaponData.autoAttack; } }
    private float cooldownExpirationTime = 0;
    public bool OffCooldown { get { return cooldownExpirationTime <= Time.time; } }
    private bool attackQueued = false;

    protected bool isCharging = false;
    protected int numCharging = 0; // This is how many charging coroutines are active at once, This allows us to ensure only one charge at a time
    private readonly bool overrideChargeUpdate = false;

    public float KnockbackStrength { get { return GetKnockbackStrength(); } }
    protected virtual float GetKnockbackStrength() { // Override this to change how strength of knockback is calculated
        return weaponData.knockbackStrength;
    }

    private Buff[] weaponBuffs;

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
        weaponBuffs = new Buff[weaponData.buffs.Length];
        for (int i = 0; i < weaponBuffs.Length; ++i) {
            weaponBuffs[i] = weaponData.buffs[i].Instance(Mathf.Infinity, this);
            player.AddBuff(weaponBuffs[i]);
        }
        Start();
        controller.OnDestroyEvent.AddListener(OnDestroyEvent);
    }
    
    public void UnequipWeapon(PlayerController controller) {
        Release();
        controller.OnDestroyEvent.RemoveListener(OnDestroyEvent);
        RemoveWeapon();
    }

    // Stop all processes in a weapon before removing
    public void RemoveWeapon() {
        foreach (Buff buff in weaponBuffs)
            player.RemoveBuff(buff);
        End();
    }

    // Start is called once when the weapon is first loaded in game use this to ensure PlayerController is active
    protected virtual void Start() { }

    public virtual void ResetCharge() { } //ChargeWeapon will use this to reset charge

    // OnDestroy is called when the player controller is destroyed;
    private void OnDestroyEvent() {
        attackQueued = false; // Make sure to remove queued attack
        isCharging = false; // Make sure to reset charging
        RemoveWeapon();
    }

    // End is called once when the weapon is removed from game
    protected virtual void End() { }

    // Activate weapon. In other words, initiate attack
    public void Activate() {
        if (!attackQueued && (!OffCooldown || AutoAttack)) { // Start queueing attacks if attack is on cooldown or AutoAttack is true
            attackQueued = true;
            player.controller.StartCoroutine(QueueingAttack());
        }
        if (OffCooldown) { // Activate if off cooldown
            cooldownExpirationTime = Time.time + weaponData.cooldown; // reset cooldown
            attackQueued = AutoAttack; // removed queued attack if auto attack is false

            if (type == Type.Charge) { // Only start charging stuff if weapon is marked as charging type weapon
                if (isCharging) // If the weapon is already charging, release the charge and reactivate
                    EndCharge();
                isCharging = true;
            }

            OnActivate();

            if (overrideChargeUpdate) // Only start OnCharginUpdate coroutine if its been overriden in derived class, This is for slight optimization
                player.controller.StartCoroutine(ChargingUpdate(numCharging));
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
            EndCharge();
        }
    }

    // Stops charging
    private void EndCharge() {
        OnRelease();
        ++numCharging;
        isCharging = false;
    }

    // OnRelease is called once when the weapon is released
    protected virtual void OnRelease() { }

    /// <summary>
    /// Handles applying damage, knockback, and other effects when a player is hit
    /// </summary>
    /// <param name="origin"> Location the hit originated from </param>
    /// <param name="target"> Object hit </param>
    /// <param name="extraData"> Extra data to be passed on to other supporting functions </param>
    protected void Hit(Vector3 origin, GameObject target, object extraData = null) {
        PlayerController targetPlayerController = target.GetComponent<PlayerController>(); // Check if target is a player
        if (targetPlayerController != null) {
            OnHit(origin, targetPlayerController, extraData); // Activate OnHit effects and get damage dealt
            targetPlayerController.player.Hurt(player, GetDamageDealt(origin, targetPlayerController, extraData)); // Hurt hit player
            Knockback(origin, targetPlayerController, extraData); // Knockback hit player
        }
    }

    // OnHit is called once when a player is hit, Return value is ammount of damage dealt
    protected virtual void OnHit(Vector3 origin, PlayerController targetPlayerController, object extraData) { }

    // GetDamageDealt returns damage
    protected virtual float GetDamageDealt(Vector3 origin, PlayerController targetPlayerController, object extraData) {
        return weaponData.damage * player.stats.Damage;
    }

    // Default knockback implementation, Knock the target back
    protected virtual void Knockback(Vector3 origin, PlayerController targetPlayerController, object extraData) {
        // Get direction relative to origin and apply knockback strength
        targetPlayerController.Knockback(((targetPlayerController.transform.position - origin).normalized + Vector3.up*0.25f).normalized * KnockbackStrength);
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
