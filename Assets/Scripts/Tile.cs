using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class Tile : MonoBehaviour {
    
    public static Vector3 TileOffset = new Vector3(0.5f, 0.5f, 0.5f);
    private bool destroying = false;

    public Vector3 position {
        get { return transform.localPosition - TileOffset; }
    }

    public void Destroy(float timer = 0) {
        if (!destroying) {
            destroying = true;
            BreakingEffect(timer);
            Invoke("DestroyEffect", timer);
        }
    }

  
    protected virtual void BreakingEffect(float duration) {
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

    public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent) {
        return Instantiate<GameObject>(prefab, pos.Round() + TileOffset, rotation, parent);
    }

    public static GameObject Create(GameObject prefab, Vector3 pos, Transform parent) {
        return Create(prefab, pos, Quaternion.identity, parent);
    }
}
