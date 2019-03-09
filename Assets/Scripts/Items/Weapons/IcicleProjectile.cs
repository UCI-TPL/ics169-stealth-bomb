﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class IcicleProjectile : Projectile {

    private Vector3 targetVelocity;
    private Rigidbody rb;
    [SerializeField]
    private float accelerationTime = 1;
    [SerializeField]
    private float accelerationPower = 2;
    private float startTime;

    protected override void Start() {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        targetVelocity = rb.velocity;
        rb.velocity = targetVelocity.normalized * 0.01f;

        origin = transform.position;
        if (player.controller.HitBox != null)
            foreach (Collider c in player.controller.HitBox)
                Physics.IgnoreCollision(c, collider);
        if (trail != null)
            trail.material.color = player.controller.playerColor;
        if (particle != null) {
            var trailModule = particle.trails;
            trailModule.colorOverLifetime = player.controller.playerColor;
        }
    }

    private void Update() {
        rb.velocity = Vector3.Lerp(Vector3.zero, targetVelocity, Mathf.Pow((Time.time - startTime) / accelerationTime, accelerationPower));
    }
}