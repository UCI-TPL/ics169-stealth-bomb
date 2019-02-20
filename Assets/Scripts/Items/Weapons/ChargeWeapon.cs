using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ChargeWeapon : Weapon {

    private ChargeWeaponData data;

    private float startChargeTime;
    protected override float GetChargeLevel() {
        return (Mathf.Min((Time.time - startChargeTime) / data.chargeTime, data.chargeLevels-1) +1)/ data.chargeLevels; //the -1 and +1 are so it goes from 1-3 instead of 0-3
    }

    Renderer rend { get { return player.controller.rend; } }

    public ChargeWeapon() : base() { }

    public ChargeWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (ChargeWeaponData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        startChargeTime = Time.time;
    }

    public override void ResetCharge() //call this to restart the charge process
    {
        isCharging = false;
        rend.material.color = player.controller.playerColor;
    }


    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        float vibrationLevel = 0.25f;
        InputManager.inputManager.controllers[player.playerNumber].Vibrate(ChargeLevel * vibrationLevel, 0.1f);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {

        GameManager.instance.audioManager.Play("Bow");
       // Debug.Log("When firing we are at : " + ChargeLevel);
        //rend.material.color = player.controller.playerColor;
        
        int chargeLevel = (int)(ChargeLevel * 100);
        data.projectile.Shoot(player, data.projSpeed * Mathf.Pow(Mathf.Floor(ChargeLevel * data.chargeLevels), 2), data.numProj, (Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target, chargeLevel); });
        //Projectile arrow = GameObject.Instantiate(data.arrow, player.controller.ShootPoint.transform.position, player.transform.rotation, null); //this instantiates the arrow as an attack
        //arrow.player = player;
        //arrow.speed = data.projSpeed * Mathf.Floor(chargeLevel);
        //arrow.damage = data.damage * Mathf.FloorToInt(chargeLevel); //should this be done differently? Perhaps we could call a function called "shoot" or "launch" and provide these variables
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        ChargeWeapon copy = new ChargeWeapon(weaponData, player);
        return copy;
    }
}
