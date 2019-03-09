using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleWeapon : Weapon {

    private IcicleWeaponData data;

    public IcicleWeapon() : base() { }

    public IcicleWeapon(WeaponData weaponData, Player player) : base(weaponData, player) {
        data = (IcicleWeaponData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        Vector3 shootPoint = player.controller.ShootPoint.transform.position;
        Vector3 spacingDirection = player.controller.ShootPoint.transform.right;
        Vector3 targetLocation = shootPoint + player.controller.ShootPoint.transform.forward * data.targetDistance;
        ProjParams[] projParams = new ProjParams[data.fireOrder.Length];
        // Set up the origin and direction for each projectile
        for (int i = 0; i < data.fireOrder.Length; ++i) {
            projParams[i].origin = shootPoint + spacingDirection * (((data.fireOrder.Length - 1) * data.bulletSpacing / -2) + data.fireOrder[i] * data.bulletSpacing);
            projParams[i].direction = Quaternion.LookRotation(targetLocation - projParams[i].origin, player.controller.ShootPoint.transform.up);
        }

        player.controller.StartCoroutine(delayedShoot(data.timeBetweenBullets));
        
        IEnumerator delayedShoot(float time) {
            float startTime = Time.time;
            for (int i = 0; i < projParams.Length; ++i) {
                data.projectile.Shoot(player, data.projSpeed, projParams[i].origin, projParams[i].direction, (Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target); });
                while (Time.time < startTime + time * (i+1))
                    yield return null;
            }
        }
    }

    struct ProjParams {
        public Vector3 origin;
        public Quaternion direction;
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        IcicleWeapon copy = new IcicleWeapon(weaponData, player);
        return copy;
    }
}
