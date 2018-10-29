using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour { //should this just be named arrow?

    // Use this for initialization

    public Rigidbody rb;

    public float speed = 1f;
    public int damage = 100;

    public float lifetime = 3f;

	void Start () {
        rb = GetComponent<Rigidbody>();
        StartCoroutine("LifeTime");
        Shoot();
    }

    void Shoot()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.HurtPlayer(damage);
            Destroy(gameObject,0.01f);
        }
    }


    // Update is called once per frame
    void Update () {

        //transform.Translate(Vector3.forward * speed);		
	}
}
