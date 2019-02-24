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


    public Transform target1;//the top of decal
    public Transform target2; //where it actually hits

    public Vector3 v2;
    public Vector3 v3;


    int vertexCount = 24;
    float ratio = 0f;

    void Start()
    {
        actualTravelTime = travelTime / vertexCount;
        startTravelTime = Time.time; //start time
        travelDuration = startTravelTime + actualTravelTime; //end time
        target = GetNextPoint();
        
    }

    
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            Tile temp = other.GetComponent<Tile>();
            if (temp)
                TileManager.tileManager.DamagePillar(temp.position, 50f); //takes 2 hits to kill stone, just one to kill grass
                //TileManager.tileManager.DestroyTiles(temp.position); //to just destory anything without caring about health
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time <= travelDuration)
        {
            float lerpPosition = (Time.time - startTravelTime) / actualTravelTime; //how far along the lerp should it be
            transform.position = Vector3.Lerp(transform.position, target, lerpPosition);
            //transform.position = GhostBomb.Parabola(transform.position, target, 2f, lerpPosition);
        }
        else
        {
            Vector3 point = GetNextPoint();
            if(point != Vector3.zero)
            {
                target = point;
                travelDuration = Time.time + actualTravelTime; //restart the lerp but going to a different part of the curve
            }   
        }
        
    }

    public Vector3 GetNextPoint() //returns the next destination on the curve
    {
        if (ratio >= 1f)
            return Vector3.zero;
        Vector3 v1 = transform.position;
        float distance = Vector3.Distance(v1, v2);
        Vector4 v4 = new Vector3(0f, 0f, 0f);
        v4 = v2 + (v1.normalized * (distance/3));
        Vector3 tangent1 = Vector3.Lerp(v1, v4, ratio); //this is the line between the ghost body & the top of the decal object
        Vector3 tangent2 = Vector3.Lerp(v4, v3, ratio); //the line between the top of the decal object & the ground
        Vector3 point = Vector3.Lerp(tangent1, tangent2, ratio); //a point on the curve that we want to make
        ratio += (1f /vertexCount);
        return point;
    }


}
