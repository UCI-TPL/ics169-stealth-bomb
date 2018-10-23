using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup List", menuName = "Power-up List", order = 1)]
public class PowerupList : ScriptableObject {
    public Tier[] tiers;

    public float TotalPercent() {
        float sum = 0;
        foreach (Tier t in tiers)
            sum += t.percent;
        return sum;
    }

    public PowerupData RandomPowerup() {
        Tier t = RandomTier();
        return t.powerups[Random.Range(0, t.powerups.Length)];
    }
    
    private Tier RandomTier() {
        float roll = Random.Range(0, TotalPercent());
        foreach (Tier t in tiers) {
            if ((roll -= t.percent) <= 0)
                return t;
        }
        Debug.LogError("Powerup tier list is empty, could not get a random powerup tier");
        return null;
    }

    [System.Serializable]
    public class Tier {
        public float percent;
        public PowerupData[] powerups;
    }
}
