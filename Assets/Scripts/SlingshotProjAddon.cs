using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotProjAddon : MonoBehaviour
{
    public Projectile proj;
    public GameObject projectileRenderer;
    public Rigidbody rb;
    public int waitFrames = 1;
    public float projSpeed = 50f;
    public int maxBounces = 2;
    private int bounceNum = 0;

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
                if (bounceNum <= maxBounces)
                {
                    proj.setHasHit(false);
                    rb.isKinematic = false;
                    Invoke("resetCollision", (float)(waitFrames/60));        // Hold off setting the flag for ~2-3 frames to let the bomb come back up.


                    if (bounceNum == maxBounces) { bombStop = true; }
                    else
                    {
                        projectileRenderer.SetActive(true);
                        rb.velocity = transform.forward * projSpeed;
                    }
                }
            }
        }
    }

    // This method is needed to prevent the bomb from going too deep into the level, and exploding twice immediately.
    // There may be one instance where the bomb phases through a corner, so if that happens, check here.
    private void resetCollision()
    {
        rb.detectCollisions = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Angle from FWD to N: " + Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up));
        //Debug.Log("Angle from BWD to N: " + Vector3.SignedAngle(-transform.forward, collision.GetContact(0).normal, transform.up));
        //Debug.Log("Cross Product (F & N): " + Vector3.Cross(transform.forward, collision.GetContact(0).normal));
        //float angle;
        /*
        if ((transform.rotation.eulerAngles.y >= 180f) && (transform.rotation.eulerAngles.y < 359.9f))
        {
            //angle = transform.rotation.eulerAngles.y - Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up) + Vector3.SignedAngle(-transform.forward, collision.GetContact(0).normal, transform.up);
            angle = transform.rotation.eulerAngles.y - Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up);
        }
        else
        {
            //angle = transform.rotation.eulerAngles.y + Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up) - Vector3.SignedAngle(-transform.forward, collision.GetContact(0).normal, transform.up);
            angle = transform.rotation.eulerAngles.y + Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up);
        }
        */

        float angle;
        angle = transform.rotation.eulerAngles.y + Vector3.SignedAngle(transform.forward, collision.GetContact(0).normal, transform.up) + Vector3.SignedAngle(-transform.forward, collision.GetContact(0).normal, transform.up);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        

    }
}
