using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    private Vector3 maxExplosionScale;
    private float growthRate;
    private float currentGrowth;
    private bool[] hitPlayers = { false, false, false, false };
    public Event_Vector3_GameObject OnHit = new Event_Vector3_GameObject();

    private void Update()
    {
        currentGrowth += growthRate;
        transform.localScale = Vector3.Lerp(Vector3.zero, maxExplosionScale, currentGrowth);
        if (currentGrowth >= 1f) { Destroy(this.gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {

       

        if (other.tag == "Player")
        {
            PlayerController oPlayer = other.gameObject.GetComponent<PlayerController>();
            if (!hitPlayers[oPlayer.player.playerNumber])
            {
                hitPlayers[oPlayer.player.playerNumber] = true;
                OnHit.Invoke(transform.position, other.transform.position, other.gameObject);
            }
        }
        else
        {
            Transform pTransform = other.transform.parent;
            if (pTransform != null)
            {
                if (pTransform.name != "Player")
                {
                    OnHit.Invoke(transform.position, other.transform.position, other.gameObject);
                    GameManager.instance.audioManager.Play("Explosion");
                }
            }
        }
    }

    public void SetUpBomb(float e_Size, float g_Rate)
    {   // someone set us up the bomb.
        maxExplosionScale = new Vector3(e_Size, e_Size, e_Size);
        growthRate = g_Rate;
        transform.localScale = Vector3.zero;
        currentGrowth = 0f;
    }
}
