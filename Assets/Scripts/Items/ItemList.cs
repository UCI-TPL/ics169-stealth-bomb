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
        return tiers[tier].RandomItem();
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
        private float[] itemWeights;
        [System.NonSerialized]
        private float addedWeight = 0;
        public float Weight { get { return baseWeight + addedWeight; } }

        public void ResetWeights() {
            addedWeight = 0;
            ResetItemWeights();
        }
        
        private void ResetItemWeights() {
            itemWeights = new float[items.Length];
            for (int i = 0; i < itemWeights.Length; ++i)
                itemWeights[i] = 1;
        }

        public float IncreaseWeight(float multiple) {
            float change = baseWeight * (multiple - 1);
            addedWeight += change;
            return change;
        }

        public void SubtractWeight(float amount) {
            addedWeight -= amount;
        }

        public ItemData RandomItem() {
            if (itemWeights == null)
                ResetItemWeights();
            float totalWeight = items.Length;
            float roll = Random.Range(0, totalWeight);
            int result = 0;
            float changePercent = 0;
            for (int itemIndex = 0; itemIndex < itemWeights.Length; ++itemIndex) {
                if ((roll -= itemWeights[itemIndex]) <= 0) {
                    result = itemIndex;

                    // calculate the change in weight required to balance out all tiers
                    itemWeights[itemIndex] -= 1f;
                    changePercent = 1f / (itemWeights.Length-1); // Decrease wight of tier if picked and save amount changed
                    break;
                }
            }
            for (int itemIndex = 0; itemIndex < itemWeights.Length; ++itemIndex) {
                if (itemIndex == result)
                    continue;
                itemWeights[itemIndex] += (1f * changePercent); // Increase the weight of all tiers proportionate to tier just picked
            }
            foreach (float i in itemWeights) {
                Debug.Log(i);
            }

            return items[result];
        }
    }
}
