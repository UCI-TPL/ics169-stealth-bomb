using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject {

    public new string name;
    public Sprite image;
    public string description;
    public Type type;

    public enum Type {
        Item, Powerup, Weapon
    }
}
