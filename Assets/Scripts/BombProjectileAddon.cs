﻿using System.Collections;
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
            time += Time.deltaTime;
            transform.position = transform.position + new Vector3(0f, Mathf.Cos(time) * Time.deltaTime, 0f) * arcHeight;


            if (proj.getHasHit() == true)
            {
                if (bounceNum < 1)
                {
                    bounceNum++;
                    rb.detectCollisions = true;
                    rb.isKinematic = false;
                    proj.transform.rotation = Quaternion.Euler(proj.transform.rotation.eulerAngles.x, proj.transform.rotation.eulerAngles.y - 90f, proj.transform.rotation.eulerAngles.z);
                    proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * 15f;
                    proj.setHasHit(false);
                }
                else
                {
                    bombStop = true;
                }
            }
        }
    }
}