using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {
    public TrailRenderer trail;

    public new Collider collider;
    [HideInInspector]
    public Player player;
    protected Vector3 origin;

    public delegate void HitAction(Vector3 origin, Vector3 contactPoint, GameObject target);
    public HitAction OnHit;

    void Start () {
        origin = transform.position;
        if (player.controller.floorCollider != null)
            Physics.IgnoreCollision(player.controller.floorCollider, collider);
        if (player.controller.wallCollider != null)
            Physics.IgnoreCollision(player.controller.wallCollider, collider);
        trail.material.color = player.controller.playerColor;
    }

    private void OnCollisionEnter(Collision other) {
        if (OnHit != null)
            OnHit.Invoke(origin, other.contacts[0].point, other.gameObject);
        Destroy(gameObject);
    }
}
