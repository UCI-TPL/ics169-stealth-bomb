using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateShield : Weapon {
    protected CreateShieldData data;

    public static readonly Dictionary<object, ShieldContainer> activeShields = new Dictionary<object, ShieldContainer>();

    public CreateShield() : base() { }

    public CreateShield(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (CreateShieldData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        if (!activeShields.ContainsKey(player.controller)) {
            activeShields.Add(player.controller, GameObject.Instantiate<GameObject>(data.shieldContainer.gameObject).GetComponent<ShieldContainer>());
            activeShields[player.controller].TargetPlayer = player.controller;
        }
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateShield copy = new CreateShield(weaponData, player);
        return copy;
    }
}
