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
        if (player.controller.HitBox != null)
            Physics.IgnoreCollision(player.controller.HitBox, collider);
        trail.material.color = player.controller.playerColor;
    }

    //private void OnTriggerEnter(Collider other) {
    //    if (other.GetComponent<PlayerController>() != null) {
    //        if (OnHit != null)
    //            OnHit.Invoke(origin, transform.position, other.gameObject);
    //        Destroy(gameObject);
    //    }
    //}

    private void OnCollisionEnter(Collision other) {
        if (OnHit != null)
            OnHit.Invoke(origin, other.contacts[0].point, other.gameObject);
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(gameObject, 3);
    }
}
