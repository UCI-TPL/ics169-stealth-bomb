using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {
    public TrailRenderer trail;

    public new Collider collider;
    [SerializeField]
    private GameObject projectileRenderer;
    [HideInInspector]
    public Player player;
    protected Vector3 origin;

    public delegate void HitAction(Vector3 origin, Vector3 contactPoint, GameObject target);
    public HitAction OnHit;

    private bool hasHit = false;

    [HideInInspector]
    public GameObject hitEffect;

    void Start () {
        origin = transform.position;
        if (player.controller.HitBox != null)
            foreach (Collider c in player.controller.HitBox)
                Physics.IgnoreCollision(c, collider);
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
        if (hasHit)
            return;
        hasHit = true;
        if (OnHit != null)
            OnHit.Invoke(origin, other.contacts[0].point, other.gameObject);
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        //GetComponent<MeshRenderer>().enabled = false;
        if (projectileRenderer != null)
            projectileRenderer.SetActive(false);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        Destroy(gameObject, 2);
    }
}
