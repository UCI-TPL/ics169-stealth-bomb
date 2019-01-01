using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR //Editor only tag
using UnityEditor;
#endif //Editor only tag
using UnityEngine;

public class SpawnTile : Tile {

    /// <summary>This determins which spawn point lower ranked players spawn. A higher priority means lower ranked players will spawn there.</summary>
    public float priority = 0;
    public override TileType Type { get { return TileType.SpawnPoint; } }

    // Destroy the Spawner as soon as the ground under it is decaying
    protected override void BreakingEffect(float duration) {
        Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.75f, 0.5f, 0.5f, 0.75f);
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        Gizmos.color = new Color(1, 0.25f, 0.25f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);

#if UNITY_EDITOR
        Handles.Label(transform.position, "Priority: " + priority.ToString());
#endif
    }
}
