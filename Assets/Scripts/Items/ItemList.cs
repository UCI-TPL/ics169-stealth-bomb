using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item List", menuName = "Item List", order = 1)]
public class ItemList : ScriptableObject {
    public Tier[] tiers;

    [System.NonSerialized]
    private int[] spawns;

    private void OnEnable() {
        ResetWeights();
    }

    public float TotalWeight() {
        float sum = 0;
        foreach (Tier t in tiers)
            sum += t.Weight;
        return sum;
    }

    public void ResetWeights() {
        foreach (Tier t in tiers)
            t.ResetWeights();
        spawns = new int[tiers.Length];
    }

    public ItemData RandomItem(int tier) {
        return tiers[tier].RandomItem();
    }

    public int RandomTier() {
        float totalWeight = TotalWeight();
        float roll = Random.Range(0, totalWeight);
        int result = -1;
        for (int tierIndex = 0; tierIndex < tiers.Length; ++tierIndex) {
            if ((roll -= tiers[tierIndex].Weight) <= 0) {
                result = tierIndex;
                ++spawns[tierIndex];
                break;
            }
        }

        int total = 0;
        foreach (int spawnCount in spawns) // Count the total number of items spawned so far
            total += spawnCount;
        for (int i = 0; i < spawns.Length; ++i) { // Rebalance item weights depending on how much the item has already spawned
            tiers[i].SetWeight(Mathf.Max(0, tiers[i].baseWeight * (1 + (tiers[i].baseWeight/100 * total - spawns[i]))));
        }

        totalWeight = TotalWeight();
        for (int i = 0; i < tiers.Length; ++i) // Rescale the weights to total to 100
            tiers[i].SetWeight(tiers[i].Weight / totalWeight * 100);

        return result;
    }

    [System.Serializable]
    public class Tier {
        public float baseWeight;
        public ItemData[] items;
        [System.NonSerialized]
        private float[] itemWeights;
        [System.NonSerialized]
        private int[] itemSpawns;
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
            itemSpawns = new int[items.Length];
        }

        public void SetWeight(float weight) {
            addedWeight = weight - baseWeight;
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


            for (int itemIndex = 0; itemIndex < items.Length; ++itemIndex) {
                if ((roll -= itemWeights[itemIndex]) <= 0) {
                    result = itemIndex;
                    ++itemSpawns[itemIndex];
                    break;
                }
            }

            int total = 0;
            foreach (int spawnCount in itemSpawns) // Count the total number of items spawned so far
                total += spawnCount;
            for (int i = 0; i < itemSpawns.Length; ++i) { // Rebalance item weights depending on how much the item has already spawned
                itemWeights[i] = Mathf.Max(0, (1 + (1 / items.Length * total - itemSpawns[i])));
            }

            totalWeight = 0;
            foreach(int i in itemWeights)
                totalWeight += i;
            for (int i = 0; i < items.Length; ++i) // Rescale the weights to total to amount of items
                itemWeights[i] = itemWeights[i] / totalWeight * items.Length;

            return items[result];
        }
    }
}
