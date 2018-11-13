using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

    public float MaxLength;
    public float Width;
    public float FrontScale = 1.5f;
    public LayerMask CollideMask;

    // Objects that make up the laserbeam effect
    public GameObject MainEffect;
    public GameObject Beam;
    public GameObject Front;

    private void LateUpdate() {
        RaycastHit hit;
        float length;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxLength, CollideMask, QueryTriggerInteraction.Ignore))
            length = Vector3.Distance(transform.position, hit.point);
        else
            length = MaxLength;
        Beam.transform.localPosition = Vector3.forward * length / 2;

        Beam.transform.localScale = new Vector3(Width, length/2, Width); // length / 2 because cylider is by default 2 units long
        Front.transform.localScale = Vector3.one * Width * FrontScale;

        MainEffect.transform.localPosition = Vector3.forward * Width / 2;
    }
}
