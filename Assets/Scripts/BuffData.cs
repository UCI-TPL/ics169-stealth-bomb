using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff/Buff", order = 2)]
public class BuffData : ScriptableObject {

    [SerializeField]
    private Buff baseBuff;
    public List<PlayerStats.Modifier> Modifiers { get { return baseBuff.Modifiers; } }
    public List<Buff.Trigger> Triggers { get { return baseBuff.Triggers; } }

    private void OnEnable() {
        if (baseBuff == null) // Set powerup data to this if instance is not yet created
            baseBuff = new Buff();
    }

    // Add modifier to the powerup
    public void AddModifier(string name, float value) {
        baseBuff.Modifiers.Add(new PlayerStats.Modifier(name, value));
    }

    // Remove modifier from the powerup
    public void RemoveModifier(int index) {
        baseBuff.Modifiers.RemoveAt(index);
    }

    public void AddTrigger() {
        baseBuff.Triggers.Add(new Buff.Trigger());
    }

    public void RemoveTrigger(int index) {
        baseBuff.Triggers.RemoveAt(index);
    }

    public Buff Instance(float duration, object source) {
        return baseBuff.DeepCopy(duration, source);
    }
}
