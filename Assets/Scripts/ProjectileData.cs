using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Weapon Props/Projectile", order = 1)]
public class ProjectileData : ScriptableObject {

    public Projectile projectile;
    
    public int damage = 100;
    public float lifetime = 3f;
    public float spreadAngle = 10;
    public int knockbackForce = 15; 

    public void Shoot(Player player, float speed, int numberOfProjectile) {
        for (int i = 0; i < numberOfProjectile; ++i)
            SpawnProjectile(player, speed, ((numberOfProjectile-1) * spreadAngle / -2) + i * spreadAngle);
    }

    private void SpawnProjectile(Player player, float speed, float rotate) {
        GameObject proj = GameObject.Instantiate(projectile.gameObject, player.controller.ShootPoint.transform.position, player.controller.transform.rotation, null); //this instantiates the arrow as an attack
        proj.GetComponent<Projectile>().player = player;
        proj.GetComponent<Projectile>().data = this;
        proj.transform.RotateAround(proj.transform.position, Vector3.up, rotate);
        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * speed;
    }
}
