using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item List", menuName = "Item List", order = 1)]
public class ItemList : ScriptableObject {
    public Tier[] tiers;

    public float TotalPercent() {
        float sum = 0;
        foreach (Tier t in tiers)
            sum += t.percent;
        return sum;
    }

    public ItemData RandomItem() {
        Tier t = RandomTier();
        return t.items[Random.Range(0, t.items.Length)];
    }
    
    private Tier RandomTier() {
        float roll = Random.Range(0, TotalPercent());
        foreach (Tier t in tiers) {
            if ((roll -= t.percent) <= 0)
                return t;
        }
        Debug.LogError("Item tier list is empty, could not get a random Item tier");
        return null;
    }

    [System.Serializable]
    public class Tier {
        public float percent;
        public ItemData[] items;
    }
}
