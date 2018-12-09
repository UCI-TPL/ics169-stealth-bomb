using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class Tile : MonoBehaviour {
    
    public static Vector3 TileOffset = new Vector3(0.5f, 0.5f, 0.5f);
    private bool destroying = false;
    public Type type { get; protected set; }

    // Set tile Type
    private void Awake() {
        type = Type.Tile;
    }
    
    /// <summary>Position of the tile snaped to the grid (nearest Int)</summary>
    public Vector3 position {
        get { return transform.localPosition - TileOffset; }
    }

    public void Destroy(float timer = 0) {
        if (!destroying) {
            destroying = true;
            BreakingEffect(timer);
            StartCoroutine(DelayDestroy(timer));
        }
    }

    private IEnumerator DelayDestroy(float timer) {
        float destroyTimer = Time.time + timer;
        while (destroyTimer > Time.time)
            yield return null;
        DestroyEffect();
    }

    /// <summary>Called once when the block begins breaking</summary>
    protected virtual void BreakingEffect(float duration) {
        GetComponent<MeshRenderer>().materials = new Material[2] { GetComponent<MeshRenderer>().material, Resources.Load<Material>("Red") };
    }

    /// <summary>Called once when the block is supposed to be destroyed. This method should destroy the Tile.</summary>
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

#if UNITY_EDITOR //Editor only tag
    /// <summary>Create the tile as a prefab in the specified position and rotation</summary>
    public static GameObject PrefabCreate(GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent) {
        GameObject tile = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
        tile.transform.position = pos.Round() + TileOffset;
        tile.transform.rotation = rotation;
        tile.transform.SetParent(parent);
        return tile;
    }
#endif //Editor only tag

    public static GameObject Create(GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent) {
        return Instantiate<GameObject>(prefab, pos.Round() + TileOffset, rotation, parent);
    }

    public static GameObject Create(GameObject prefab, Vector3 pos, Transform parent) {
        return Create(prefab, pos, Quaternion.identity, parent);
    }

    public enum Type {
        Tile, Item, SpawnPoint
    }
}
