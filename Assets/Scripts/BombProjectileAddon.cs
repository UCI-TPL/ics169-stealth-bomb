using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectileAddon : MonoBehaviour
{
    public Projectile proj;
    public Rigidbody rb;
    //public int maxBounces = 1;
    private int bounceNum = 0;
    public float arcHeight = 3.5f;
    private float time = 0f;
    private bool bombStop = false;

    void Awake()
    {
        if (proj == null)
            proj = gameObject.GetComponent<Projectile>();
        if (rb == null)
            rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!bombStop)
        {
            if (proj.getHasHit() == true)
            {
                bounceNum++;
                if (bounceNum <= 2)
                {
                    proj.setHasHit(false);
                    rb.detectCollisions = true;
                    rb.isKinematic = false;

                    if (bounceNum == 2) { bombStop = true; }
                    else
                    {
                        proj.transform.rotation = Quaternion.Euler(proj.transform.rotation.eulerAngles.x, proj.transform.rotation.eulerAngles.y - 90f, proj.transform.rotation.eulerAngles.z);
                        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * 8f;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!bombStop)
        {
            time += Time.deltaTime;
            Vector3 movement = transform.position + new Vector3(0f, Mathf.Cos(time) * Time.deltaTime, 0f) * arcHeight;
            rb.MovePosition(movement);
        }
    }
}
