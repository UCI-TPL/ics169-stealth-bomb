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
    private Material BeamMaterial;
    public GameObject Front;
    private Material FrontMaterial;

    private void Awake() {
        BeamMaterial = Beam.GetComponent<Renderer>().material;
        if (BeamMaterial.shader.name != "LaserShader")
            Debug.Log(name + " has incorrect shader, LaserShader required.");
        FrontMaterial = Front.GetComponent<Renderer>().material;
        if (FrontMaterial.shader.name != "LaserShader")
            Debug.Log(name + " has incorrect shader, LaserShader required.");
    }

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

        BeamMaterial.SetFloat("Vector1_5DB383F", length / Width / 4);

        MainEffect.transform.localPosition = Vector3.forward * Width / 2;
    }

    public void SetColor(Color color) {
        Vector4 HDRColor = color * 4;
        BeamMaterial.SetColor("Color_E025656E", HDRColor);
        FrontMaterial.SetColor("Color_E025656E", HDRColor);
    }
}
