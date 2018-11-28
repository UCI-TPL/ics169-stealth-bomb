using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item List", menuName = "Item List", order = 1)]
public class ItemList : ScriptableObject {
    public Tier[] tiers;

    public float TotalWeight() {
        float sum = 0;
        foreach (Tier t in tiers)
            sum += t.Weight;
        return sum;
    }

    public ItemData RandomItem(int tier) {
        return tiers[tier].items[Random.Range(0, tiers[tier].items.Length)];
    }
    
    public int RandomTier() {
        float totalWeight = TotalWeight();
        float roll = Random.Range(0, totalWeight);
        int result = -1;
        float changePercent = 0;
        for (int tierIndex = 0; tierIndex < tiers.Length; ++tierIndex) {
            if ((roll -= tiers[tierIndex].Weight) <= 0) {
                result = tierIndex;

                // calculate the change in weight required to balance out all tiers
                float totalWeightExclude = totalWeight - tiers[tierIndex].baseWeight;
                tiers[tierIndex].SubtractWeight(tiers[tierIndex].baseWeight);
                changePercent = tiers[tierIndex].baseWeight / totalWeightExclude; // Decrease wight of tier if picked and save amount changed
                break;
            }
        }
        for (int tierIndex = 0; tierIndex < tiers.Length; ++tierIndex) {
            if (tierIndex == result)
                continue;
            tiers[tierIndex].IncreaseWeight(1 + changePercent); // Increase the weight of all tiers proportionate to tier just picked
        }
        return result;
    }

    [System.Serializable]
    public class Tier {
        public float baseWeight;
        public ItemData[] items;
        [System.NonSerialized]
        private float addedWeight = 0;
        public float Weight { get { return baseWeight + addedWeight; } }

        public void ResetWeight() {
            addedWeight = 0;
        }

        public float IncreaseWeight(float multiple) {
            float change = baseWeight * (multiple - 1);
            addedWeight += change;
            return change;
        }

        public void SubtractWeight(float amount) {
            addedWeight -= amount;
        }
    }
}
