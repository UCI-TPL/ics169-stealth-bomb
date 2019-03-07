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
    private int layerMask;
    void Awake()
    {
        if (proj == null)
            proj = gameObject.GetComponent<Projectile>();
        if (rb == null)
            rb = gameObject.GetComponent<Rigidbody>();
        int playerMask = 1 << 9;        // Hit anything on the player layer.
        int groundMask = 1 << 11;       // Hit anything on the ground layer.
        layerMask = playerMask | groundMask;
    }

    void Update()
    {
        if (rb.velocity != transform.forward * projSpeed)
        {
            rb.velocity = transform.forward * projSpeed;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Time.deltaTime * projSpeed + .05f, layerMask))
        {
            Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
            float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            rb.velocity = transform.forward * projSpeed;
        }


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
}
