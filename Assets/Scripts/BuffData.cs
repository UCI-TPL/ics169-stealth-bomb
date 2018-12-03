using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffData<BuffType> : BuffData where BuffType : Buff, new() {

    [SerializeField]
    protected BuffType baseBuff;
    public override List<PlayerStats.Modifier> Modifiers { get { return baseBuff.Modifiers; } }
    public override List<Buff.Trigger> Triggers { get { return baseBuff.Triggers; } }

    private void OnEnable() {
        if (baseBuff == null) // Set powerup data to this if instance is not yet created
            baseBuff = new BuffType();
    }

    // Add modifier to the powerup
    public override void AddModifier(string name, float value) {
        baseBuff.Modifiers.Add(new PlayerStats.Modifier(name, value));
    }

    // Remove modifier from the powerup
    public override void RemoveModifier(int index) {
        baseBuff.Modifiers.RemoveAt(index);
    }

    public override void AddTrigger() {
        baseBuff.Triggers.Add(new Buff.Trigger());
    }

    public override void RemoveTrigger(int index) {
        baseBuff.Triggers.RemoveAt(index);
    }

    public override Buff Instance(float duration, object source) {
        return baseBuff.DeepCopy(this, duration, source);
    }
}

public abstract class BuffData : ScriptableObject {

    public abstract List<PlayerStats.Modifier> Modifiers { get; }
    public abstract List<Buff.Trigger> Triggers { get; }

    // Add modifier to the powerup
    public abstract void AddModifier(string name, float value);

    // Remove modifier from the powerup
    public abstract void RemoveModifier(int index);

    public abstract void AddTrigger();

    public abstract void RemoveTrigger(int index);

    public abstract Buff Instance(float duration, object source);
}
