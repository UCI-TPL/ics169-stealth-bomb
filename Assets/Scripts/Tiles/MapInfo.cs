using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour //this script will be attached to the TileMap Empty and will have any important info for that map
{
    //The first important thing that needs to be recorded is the position of the Ghost Curve and such things 


    [Header("Ghost Curve Positions")]
    //might be expanded in the future but for now we only need two curves 
    public Curve LeftCurve;

    public Curve RightCurve;

    [Header("Limits for GhostPlayer")]
    public float max;
    public float min;

}


[System.Serializable]
public class Curve
{
    [Tooltip("Like either Curve 1 or Curve 2")]
    public string Name;

    public Vector3 Start;
    public Vector3 Middle;
    public Vector3 End; //the positions of the GhostCurve pieces

}