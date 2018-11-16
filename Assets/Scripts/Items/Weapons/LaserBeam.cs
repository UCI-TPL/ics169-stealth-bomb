using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

    public float MaxLength;
    public float Width;
    public float FrontScale = 1.5f;
    public float Speed = 4;
    public LayerMask CollideMask;
    public float particleEmiterScale = 1.25f;
    public float particlesPerLength = 10;

    // Objects that make up the laserbeam effect
    public GameObject mainEffect;
    public GameObject beam;
    private Material beamMaterial;
    public GameObject front;
    private Material frontMaterial;
    public ParticleSystem beamParticleSystem;

    private void Awake() {
        beamMaterial = beam.GetComponent<Renderer>().material;
        if (beamMaterial.shader.name != "LaserShader")
            Debug.Log(name + " has incorrect shader, LaserShader required.");
        frontMaterial = front.GetComponent<Renderer>().material;
        if (frontMaterial.shader.name != "LaserShader")
            Debug.Log(name + " has incorrect shader, LaserShader required.");
    }

    private void LateUpdate() {
        mainEffect.transform.localPosition = Vector3.forward * Width / 2; // Move Beam forward slightly to account for width of ball in front

        RaycastHit hit;
        float length;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxLength, CollideMask, QueryTriggerInteraction.Ignore))
            length = Vector3.Distance(transform.position, hit.point) - mainEffect.transform.localPosition.magnitude; // subtract distance beam moved forward in earlier step
        else
            length = MaxLength;
        beam.transform.localPosition = Vector3.forward * length / 2;

        beam.transform.localScale = new Vector3(Width, length, Width);
        front.transform.localScale = Vector3.one * Width * FrontScale;

        beamMaterial.SetFloat("Vector1_5DB383F", length / Width / 2);
        beamMaterial.SetFloat("Vector1_81F1F082", Speed);
        frontMaterial.SetFloat("Vector1_81F1F082", Speed/2);

        UpdateParticles(length, Width / 2);
    }

    public void EnableParticles() {
        beamParticleSystem.Play();
    }

    public void DisableParticles() {
        beamParticleSystem.Stop();
    }

    private void OnDisable() {
        UpdateParticles(0, 0);
    }

    private void UpdateParticles(float length, float radius) {
        var shape = beamParticleSystem.shape;
        shape.length = Mathf.Max(length - 5, 0);
        shape.radius = radius * particleEmiterScale;

        var emission = beamParticleSystem.emission;
        emission.rateOverTime = length * particlesPerLength;
    }

    public void SetColor(Color color) {
        Vector4 HDRColor = color * 2;
        beamMaterial.SetColor("Color_E025656E", HDRColor);
        frontMaterial.SetColor("Color_E025656E", HDRColor);
        var main = beamParticleSystem.main;
        main.startColor = color;
    }
}
