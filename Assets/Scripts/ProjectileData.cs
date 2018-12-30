using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Weapon Props/Projectile", order = 1)]
public class ProjectileData : ScriptableObject {

    public Projectile projectile;
    public GameObject hitEffect;

    public float lifetime = 3f;
    public float spreadAngle = 10;

    public bool rotate90;

    public void Shoot(Player player, float speed, int numberOfProjectile, Projectile.HitAction Onhit) {
        for (int i = 0; i < numberOfProjectile; ++i)
            SpawnProjectile(player.controller.ShootPoint.transform.position, player.controller.ShootPoint.transform.rotation * Quaternion.AngleAxis(((numberOfProjectile - 1) * spreadAngle / -2) + i * spreadAngle, player.controller.ShootPoint.transform.up), speed, Onhit, player);
    }

    private void SpawnProjectile(Vector3 origin, Quaternion direction, float speed, Projectile.HitAction Onhit, Player player) {

        Projectile proj = GameObject.Instantiate(projectile.gameObject, origin, direction).GetComponent<Projectile>(); //this instantiates the arrow as an attack
        proj.OnHit = Onhit;
        proj.player = player;
        proj.hitEffect = hitEffect;
        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * speed;

        if(rotate90)
        {
            proj.transform.rotation = direction * Quaternion.Euler(0f, 90f, 0f);
        }

        Destroy(proj.gameObject, lifetime);
    }
}
