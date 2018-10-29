using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeWeapon : Weapon {

    private ChargeWeaponData data;

    private float startChargeTime;
    public float chargeLevel {
        get { return Mathf.Min((Time.time - startChargeTime) / data.chargeTime, data.chargeLevels-1) +1; } //the -1 and +1 are so it goes from 1-3 instead of 0-3
    }

    Renderer rend;
    Color startColor;

    public ChargeWeapon() : base() { }

    public ChargeWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (ChargeWeaponData)weaponData;
        rend = player.GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        startChargeTime = Time.time;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        switch((int)chargeLevel)
        {
            case 1:
                rend.material.color = rend.material.color + ((Color.red / data.colorAddition) * Time.deltaTime);
                break;
            case 2:
                rend.material.color = rend.material.color +  (Color.magenta / data.colorAddition)  * Time.deltaTime;
                break;
            case 3:
                //rend.material.color = rend.material.color + (Color.green / data.colorAddition)  * Time.deltaTime;
                break;
            default: //no color change after a certain point
                break;

        }
        //rend.material.color = rend.material.color + (Color.red / data.colorAddition);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        rend.material.color = startColor;
        Debug.Log("Releasing with a chargeTime of " + chargeLevel);
        //if (chargeLevel >= 1) { }
        for (int i = 0; i < data.numProj; ++i)
        {
            Projectile arrow = GameObject.Instantiate(data.arrow, player.controller.ShootPoint.transform.position, player.transform.rotation, null); //this instantiates the arrow as an attack
            arrow.speed = data.projSpeed * chargeLevel;
            arrow.damage = data.damage * (int)chargeLevel; //should this be done differently? Perhaps we could call a function called "shoot" or "launch" and provide these variables
        }


        
    }

    // Create a deep copy of this weapon instance
    public override Weapon Clone(WeaponData data, Player player) {
        ChargeWeapon copy = new ChargeWeapon(data, player);
        return copy;
    }
}
