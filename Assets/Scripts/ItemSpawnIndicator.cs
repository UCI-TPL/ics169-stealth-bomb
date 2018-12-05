using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnIndicator : MonoBehaviour {

    public Color color;
    public Material particleMaterial;

    public ItemContainer itemContainer;
    private ItemData data;

    [Header("Script Referances")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private ParticleSystem[] particles;
    [SerializeField]
    private ParticleSystemRenderer[] particleMaterials;
    [SerializeField]
    private Animator animator;

    // Use this for initialization
    void Start () {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[lineRenderer.colorGradient.colorKeys.Length];
        for (int i = 0; i < lineRenderer.colorGradient.colorKeys.Length; ++i) { // Set the Line Renderer's Color
            colorKey[i].color = color;
            colorKey[i].time = lineRenderer.colorGradient.colorKeys[i].time;
        }
        gradient.SetKeys(colorKey, lineRenderer.colorGradient.alphaKeys);
        lineRenderer.colorGradient = gradient;

        gradient.SetKeys(colorKey, trailRenderer.colorGradient.alphaKeys);
        trailRenderer.colorGradient = gradient;

        foreach (ParticleSystem p in particles) { // Set the Particle Systems' Colors
            ParticleSystem.MainModule m = p.main;
            m.startColor = color;
        }

        foreach (ParticleSystemRenderer p in particleMaterials) { // Set the Particle Systems' Materials
            p.material = particleMaterial;
        }
    }
	
    public void StartTimer(ItemData data) {
        gameObject.SetActive(true);
        this.data = data;
        animator.Play("ItemSpawn");
    }

    public void SpawnItem() { // Spawns the item at the indicator's location
        GameObject g = Instantiate(itemContainer.gameObject, transform.position, Quaternion.identity);
        g.GetComponent<ItemContainer>().SetItemData(data);
        Destroy(gameObject, 1);
    }
}
