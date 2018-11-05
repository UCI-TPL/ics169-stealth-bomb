using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {
    public Rigidbody rb;
    public TrailRenderer trail;

    public float speed = 1f;
    public int damage = 100;

    public new Collider collider;
    public ProjectileData data;
    [HideInInspector]
    public Player player;

	void Start () {
        Physics.IgnoreCollision(player.controller.floorCollider, collider);
        Physics.IgnoreCollision(player.controller.wallCollider, collider);
        Destroy(gameObject, data.lifetime); // Destroy gameObject after lifetime is up
        trail.material.color = player.playerColor;
    }

    private void OnCollisionEnter(Collision other) {
        Player hit;
        if ((hit = other.gameObject.GetComponent<Player>()) != null && hit != player) {
            hit.HurtPlayer(data.damage);
            Debug.Log("What they get" + transform.forward * data.knockbackForce);
            hit.Knockback(transform.forward * data.knockbackForce); //the knockback will be in the direction of the projectile 
            Destroy(gameObject);
        }
    }
}
