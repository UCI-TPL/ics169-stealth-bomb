using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostBomb : MonoBehaviour
{
    //This bomb is created and then it flies towards its target

    [HideInInspector]
    public Vector3 target; //where it flies to

    [SerializeField]
    public int Damage;

  
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


    private Vector3 StartPosition;


    bool hitSomething = false;

    int vertexCount = 24;
    float ratio = 0f;

    public bool started = false;

    private Rigidbody rb;

    private float timeElapsed = 0f;

    private Keyframe[] XKeys;
    private AnimationCurve X;

    private Keyframe[] ZKeys;
    private AnimationCurve Z;

    private Keyframe[] YKeys;
    private AnimationCurve Y;

    private Vector3 forward;
    private Vector3 right;


    void Start()
    {
        actualTravelTime = travelTime / vertexCount; //how long travelling with actually take
        startTravelTime = Time.time; //start time
        travelDuration = startTravelTime + actualTravelTime; //end time
        rb = GetComponent<Rigidbody>();
        StartPosition = transform.position;

    }

    public void Initialize(Vector3 t2, Vector3 t3) //where the two targets are set
    {
        v2 = t2; // ghost cursor object
        v3 = t3; //the ground
        target = GetNextPoint();
        started = true;

        XKeys = new Keyframe[2];
        XKeys[0] = new Keyframe(0f, StartPosition.x);
        XKeys[1] = new Keyframe(1f, t3.x);

        ZKeys = new Keyframe[2];
        ZKeys[0] = new Keyframe(0f, StartPosition.z);
        ZKeys[1] = new Keyframe(1f, t3.z);

        YKeys = new Keyframe[2];
        YKeys[0] = new Keyframe(0f, StartPosition.y);
        YKeys[1] = new Keyframe(1f, t3.y);




        timeElapsed = 0f;

        X = new AnimationCurve(XKeys);
        Z = new AnimationCurve(ZKeys);
        Y = new AnimationCurve(YKeys);

        
    }

    private void OnTriggerEnter(Collider other)
    {

        //if (!started) //not sure if this does anything but whatever
        //    return;
        LayerMask mask = LayerMask.GetMask("Ground");

        if (other.gameObject.layer == 11) //prevents and funny business from going on 
        {
            
            DamageTiles();
        }
    }

    public void DamageTiles(float explosionDepth = 1f)
    {
        if (hitSomething) //prevents this function from being called twice. This is final
            return;
        hitSomething = true;
        LayerMask mask = LayerMask.GetMask("Ground");
        Vector3 ColliderZone = new Vector3(transform.localScale.x, 1f, transform.localScale.x);
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, ColliderZone, Quaternion.identity, mask);

        int i = 0;
        while (i < hitColliders.Length)
        {

            Tile temp = hitColliders[i].GetComponent<Tile>();
            if (temp != null)
            {
                temp.ApplyDamage(Damage);
             
            }
            else
            {
                //Debug.Log("Temp is null? Why can't the tile be found  : "+hitColliders[i].gameObject.name);
                Destroy(hitColliders[i].gameObject);
            }

            
            i++;
        }
        
        //SecondaryDamage();
        Explode();
    }

    public void SecondaryDamage() //for some reasons tiles weren't getting damaged when exploding next to it. This is a smaller explosion that DELETES THINGS
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        Vector3 ColliderZone = new Vector3(2f, 2f, 2f);
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, ColliderZone, Quaternion.identity, mask);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Destroy(hitColliders[i].gameObject); //kill it all
            i++;
        }
    }

    public void Explode() //the final action
    {
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



    private void UpdateCameraDirection()
    {
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();
    }

    // Update is called once per frame
    void FixedUpdate()
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
                if(!hitSomething)
                    DamageTiles();
                //Destroy(this.gameObject);
            }
        }
        float lerpPosition = (Time.time - startTravelTime) / actualTravelTime; //how far along the lerp should it be
        rb.position = Vector3.Lerp(transform.position, target, lerpPosition);
        


        /*  trying and failing to use animation curves
        UpdateCameraDirection();
        timeElapsed += Time.deltaTime; //tried to move thigs with animation cureves

        float moveX = X.Evaluate(timeElapsed);
        float moveZ = Z.Evaluate(timeElapsed);
        float moveY = Y.Evaluate(timeElapsed);

        Vector3 pos = new Vector3(StartPosition.x + moveX, StartPosition.y + moveY, StartPosition.z + moveZ);

        Vector3 scaledVector = (pos.y * forward) + (pos.x * right);

        Debug.Log("We got " + pos);
        Debug.Log("And scaled vecotr " + scaledVector);

        rb.MovePosition(scaledVector);
        */

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
