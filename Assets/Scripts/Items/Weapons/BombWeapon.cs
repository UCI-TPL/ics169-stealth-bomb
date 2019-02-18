using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeapon : Weapon {
    private BombWeaponData data;
    private int maxUses;
    private int timesUsed;
    private bool alreadyHit;                    // Prevents multiple explosions from generating at one time.


    public BombWeapon() : base() { }
    public BombWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Temp) {
        base.isCharging = true;
        data = (BombWeaponData)weaponData;
        timesUsed = 0;
    }

    protected override void Start() {
        maxUses = data.numberOfUses;
    }

    protected override float GetChargeLevel()       // override GetChargeLevel to get the amount of ammo left.
    {
        return 1f - ((float)timesUsed / (float)maxUses);
    }

    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null)
    {
        if (timesUsed < maxUses)
        {
            timesUsed++;
            alreadyHit = false;
            data.projectile.Shoot(player, data.projSpeed, data.projNum, (Vector3 origin, Vector3 contactPoint, GameObject target) => { CreateBomb(origin, contactPoint); });
            if (timesUsed == maxUses)
            {
                if (player.controller.PreviousWeapon == null)           // Workaround for when a player holds onto a Bomb into a new round.
                {
                    player.ResetWeapon();
                    player.controller.PreviousWeapon = player.controller.Weapon;
                }
                else
                {
                    player.ChangeWeapon(player.controller.PreviousWeapon.weaponData);
                    player.controller.PreviousWeapon = player.controller.Weapon;
                }
            }
        }
    }

    protected override void OnHit(Vector3 origin, Vector3 contactPoint, PlayerController targetPlayerController, object extraData)
    {
        if (!alreadyHit)
        {
            GameObject.Instantiate(data.bombPrefab, (Vector3)extraData, Quaternion.identity);
            alreadyHit = true;
        }
    }

    protected override void Knockback(Vector3 origin, PlayerController targetPlayerController, object extraData)
    {
        targetPlayerController.Knockback(((targetPlayerController.transform.position - origin).normalized + Vector3.up * -0.25f).normalized * KnockbackStrength, player.playerNumber);

    }

    protected void CreateBomb(Vector3 originBW, Vector3 spawnPoint)
    {
        Bomb bomb = GameObject.Instantiate(data.bombPrefab, spawnPoint, Quaternion.identity).GetComponent<Bomb>();
        bomb.SetUpBomb(data.explosionSize, data.growthRate);
        // BombWeapon defines Hit. Bomb has an event, which Bomb adds a lambda(?) to as a listener, which is a wrapper for the Hit method defined by Weapon.
        bomb.OnHit.AddListener((Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(originBW, contactPoint, target, ChargeLevel); });
        alreadyHit = true;
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        BombWeapon copy = new BombWeapon(weaponData, player);
        return copy;
    }
}
