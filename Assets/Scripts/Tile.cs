using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public GameObject prefab;
    private bool destroying = false;

    public void Destroy(float timer = 0) {
        if (!destroying) {
            destroying = true;
            BreakingEffect();
            Invoke("DestroyEffect", timer);
        }
    }

  
    protected virtual void BreakingEffect() {
        GetComponent<MeshRenderer>().materials = new Material[2] { GetComponent<MeshRenderer>().material, Resources.Load<Material>("Red") };
    }

    protected virtual void DestroyEffect() {
        StartCoroutine("DefaultDestroyEff");
    }

    private IEnumerator DefaultDestroyEff() {
        GetComponent<Collider>().enabled = false;
        float curTime = Time.time;
        while (true) {
            float scale = -Mathf.Pow((4f * (Time.time - curTime - 0.125f)), 2) + 1.25f;
            transform.localScale = new Vector3(scale, scale, scale);
            if (scale <= 0)
                break;
            yield return null;
        }
        Destroy(gameObject);
    }
}
