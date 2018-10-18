using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Use this for initialization

    public int speed = 1;
    public int damage = 100;

    public float lifetime = 3f;

	void Start () {
        StartCoroutine("LifeTime");
	}

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.HurtPlayer(damage);
        }
    }


    // Update is called once per frame
    void Update () {

        transform.Translate(Vector3.forward * speed);		
	}
}
