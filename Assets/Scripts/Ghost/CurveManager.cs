using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveManager : MonoBehaviour //This exists for GameManager can talk to the Curves. This is seperate from the other Managers and lives in 
{

    public ParabolaController Curve1;
    public ParabolaController Curve2;


    public void ResetCurve1(Curve curve )
    {
        Curve1.transform.Find("Start").transform.localPosition = curve.Start;
        Curve1.transform.Find("Middle").transform.localPosition = curve.Middle;
        Curve1.transform.Find("End").transform.localPosition = curve.End;
        Curve1.ParabolaRoot = Curve1.gameObject;
        Curve1.Reset();
    }

    public void ResetCurve2(Curve curve)
    {
        Curve2.transform.Find("Start").transform.localPosition = curve.Start;
        Curve2.transform.Find("Middle").transform.localPosition = curve.Middle;
        Curve2.transform.Find("End").transform.localPosition = curve.End;
        Curve2.ParabolaRoot = Curve2.gameObject;
        Curve2.Reset();
    }

}
