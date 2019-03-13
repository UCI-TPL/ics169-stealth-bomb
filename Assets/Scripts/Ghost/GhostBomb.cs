using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostBomb : MonoBehaviour
{
    //This bomb is created and then it flies towards its target

    [HideInInspector]
    public Vector3 target; //where it flies to

  
    public float travelTime; //how long it should travel

    private float actualTravelTime;

    [HideInInspector]
    public float travelDuration; //how long it has travelled

    [HideInInspector]
    public float startTravelTime;

    [HideInInspector]
    public Transform target1;//the top of decal
    [HideInInspector]
    public Transform target2; //where it actually hits

    [HideInInspector]
    public Vector3 v2; //travel destinations
    [HideInInspector]
    public Vector3 v3;

    private bool explosionPlayed = false; //to make sure only one explosion spawns
    public GameObject ExplosionParticles;


    int vertexCount = 24;
    float ratio = 0f;

    public bool started = false;

    void Start()
    {
        actualTravelTime = travelTime / vertexCount; //how long travelling with actually take
        startTravelTime = Time.time; //start time
        travelDuration = startTravelTime + actualTravelTime; //end time

    }

    public void Initialize(Vector3 t2, Vector3 t3) //where the two targets are set
    {
        
        v2 = t2; // ghost cursor object
        v3 = t3; //the ground
        target = GetNextPoint();
        started = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!started)
            return;
        LayerMask mask = LayerMask.GetMask("Ground");

        if (other.gameObject.layer != 11) //prevents and funny business from going on 
            return;

        Vector3 ColliderZone = new Vector3(transform.localScale.x, transform.localScale.y * 10f, transform.localScale.x);

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, ColliderZone, Quaternion.identity, mask);

            int i = 0;

            
        while (i < hitColliders.Length)
        {

            Tile temp = hitColliders[i].GetComponent<Tile>();
               if (temp)
                {
                    if (hitColliders[i].tag == "Tile")
                        TileManager.tileManager.DamagePillar(temp.position, 50f); //takes 2 hits to kill stone, just one to kill grass
                    else
                        Destroy(hitColliders[i].gameObject);
                }
                else
                {
                    Destroy(hitColliders[i].gameObject);
                }

            Debug.Log("Dying after colliding with " + hitColliders[i].gameObject.name);
                Explode(); //goodybye
                i++;
        }
        //}

        /*

        if (other.gameObject.layer == 11) //layer 11 is ground
        {
            Tile temp = other.GetComponent<Tile>();
            if (temp)
            {
                if (other.tag == "Tile")
                    TileManager.tileManager.DamagePillar(temp.position, 50f); //takes 2 hits to kill stone, just one to kill grass
                else
                    Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
            Explode(); //goodybye
        }*/
    }

    void Explode() //the final action
    {

        Debug.Log("Travel time is " + actualTravelTime);
        if (!started)
            return;
        if (!explosionPlayed)
        {
            GameObject explosion = Instantiate(ExplosionParticles, transform.position, Quaternion.identity);
            explosion.GetComponent<ParticleSystem>().Play();
            explosionPlayed = true;
        }

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
            return;
        if (Time.time > travelDuration)
        {
            Vector3 point = GetNextPoint();
            if (point != Vector3.zero) {
                target = point;
                travelDuration = Time.time + actualTravelTime; //restart the lerp but going to a different part of the curve
            }
            if(point == Vector3.zero)
            {
                Debug.Log("The journey ends now");
            }
        }


        

        float lerpPosition = (Time.time - startTravelTime) / actualTravelTime; //how far along the lerp should it be
        //Debug.Log("Target is " + target);
        transform.position = Vector3.Lerp(transform.position, target, lerpPosition);


        if (Time.time > travelDuration)
        {
            Debug.Log("Travel duration is " + travelDuration);
            Explode();
        }


    }

    public Vector3 GetNextPoint() //returns the next destination on the curve
    {
        if (ratio >= 1f)
            return Vector3.zero;
        Vector3 v1 = transform.position;
        float distance = Vector3.Distance(v1, v2);
        Vector4 v4 = new Vector3(0f, 0f, 0f);
        v4 = v2 + (v1.normalized * (distance/3)); //this is there to make the curve smoother
        Vector3 tangent1 = Vector3.Lerp(v1, v4, ratio); //this is the line between the ghost body & the top of the decal object
        Vector3 tangent2 = Vector3.Lerp(v4, v3, ratio); //the line between the top of the decal object & the ground
        Vector3 point = Vector3.Lerp(tangent1, tangent2, ratio); //a point on the curve that we want to make
        ratio += (1f /vertexCount); //the increment
        return point;
    }


}
