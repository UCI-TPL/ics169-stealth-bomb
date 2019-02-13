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

    [HideInInspector]
    public float travelDuration; //how long it has travelled

    [HideInInspector]
    public float startTravelTime;

    void Start()
    {
        startTravelTime = Time.time; //start time
        travelDuration = startTravelTime + travelTime; //end time
        
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
            float lerpPosition = (Time.time - startTravelTime) / travelTime; //how far along the lerp should it be
            //transform.position = Vector3.Lerp(transform.position, target, lerpPosition);
            transform.position = GhostBomb.Parabola(transform.position, target, 2f, lerpPosition);
        }
        
    }
}
