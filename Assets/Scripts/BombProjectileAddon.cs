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
    public float arcMoveSpeed = 2f;
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
                Debug.Log("Counting Bounce");
                bounceNum++;
                if (bounceNum <= 2)
                {
                    proj.setHasHit(false);
                    rb.isKinematic = false;
                    Invoke("resetCollision", .042f);        // Hold off setting the flag for ~2-3 frames to let the bomb come back up.


                    if (bounceNum == 2) { bombStop = true; }
                    else
                    {
                        proj.transform.rotation = Quaternion.Euler(proj.transform.rotation.eulerAngles.x, proj.transform.rotation.eulerAngles.y - 90f, proj.transform.rotation.eulerAngles.z);
                        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * 12f;
                        time = 0f;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!bombStop)
        {
            time += Time.deltaTime * arcMoveSpeed;
            Vector3 movement = transform.position + new Vector3(0f, Mathf.Cos(time) * Time.deltaTime, 0f) * arcHeight;
            rb.MovePosition(movement);
        }
    }

    // This method is needed to prevent the bomb from going too deep into the level, and exploding twice immediately.
    // There may be one instance where the bomb phases through a corner, so if that happens, check here.
    private void resetCollision()
    {
        rb.detectCollisions = true;
    }
}
