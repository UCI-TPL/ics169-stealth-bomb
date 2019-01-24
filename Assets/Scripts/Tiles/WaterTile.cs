using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile {

    protected override void BreakingEffect(float duration) {
    }

    protected override void DestroyEffect() {
        StartCoroutine(DisolveEffect(0.5f));
    }

    private IEnumerator DisolveEffect(float duration) {
        float startTime = Time.time;
        float endTime = Time.time + duration;
        while (endTime >= Time.time) {
            transform.position += Vector3.down * (1 - endTime + startTime); // Set shader dissolve level
            yield return null;
        }
        Destroy(gameObject);
    }
}
