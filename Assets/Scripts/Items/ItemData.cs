using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject {

    public new string name;
    public Sprite image;
    public string description;
    [HideInInspector]
    public Type type;

    private void OnEnable() {
        type = ItemData.Type.Item;
    }

    public abstract void Use(Player player);

    public enum Type {
        Item, Powerup, Weapon, SpecialMove
    }
}
