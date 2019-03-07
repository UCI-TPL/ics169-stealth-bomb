using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldContainer : MonoBehaviour
{
    private PlayerController targetPlayer;
    public PlayerController TargetPlayer { get { return targetPlayer; } set { SetTarget(value); } }
    public float rotationSpeed = 180;

    public CreateShieldData shieldData;
    private Shield[] shields;
    [SerializeField]
    private string counterName;

    private void Awake() {
        shields = GetComponentsInChildren<Shield>();
    }

    // Update is called once per frame
    void LateUpdate() {
        if (targetPlayer == null) {
            Destroy(gameObject);
            return;
        }

        float shieldCount = TargetPlayer.player.stats.GetStat(counterName);
        for (int i = 0; i < shields.Length; ++i)
            shields[i].gameObject.SetActive(i < shieldCount);
        
        transform.position = targetPlayer.transform.position;
        transform.Rotate(transform.up * rotationSpeed * Mathf.Deg2Rad);
    }

    private void SetTarget(PlayerController targetPlayer) {
        if (this.targetPlayer != null)
            foreach (Shield shield in shields)
                foreach (Collider c in shield.colliders)
                    this.targetPlayer.HitBox.Remove(c);
        foreach (Shield shield in shields)
            foreach (Collider c in shield.colliders)
                targetPlayer.HitBox.Add(c);
        this.targetPlayer = targetPlayer;

        foreach (Shield s in shields)
            s.SetMaterial(shieldData.PlayerColorMaterials[targetPlayer.player.colorIndex % shieldData.PlayerColorMaterials.Length]); // We mod the index to to make sure there is never out of index error, instead it fails in a more acceptable manner, by using the wrong color
    }
}
