﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {

    public new Collider collider;
    public ProjectileData data;
    [HideInInspector]
    public Player player;

	void Start () {
        Physics.IgnoreCollision(player.GetComponent<Collider>(), collider);
        Destroy(gameObject, data.lifetime); // Destroy gameObject after lifetime is up
    }

    private void OnCollisionEnter(Collision other) {
        Player hit;
        if ((hit = other.gameObject.GetComponent<Player>()) != null && hit != player) {
            hit.HurtPlayer(data.damage);
            Destroy(gameObject);
        }
    }
}
