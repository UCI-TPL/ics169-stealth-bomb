using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DodgeRoll : Weapon
{

    private DodgeData data;

    float cooldown = 0.0f;

    public DodgeRoll() : base() { }

    public DodgeRoll(WeaponData weaponData, Player player) : base(weaponData, player, Type.Move)
    {
        data = (DodgeData)weaponData;
    }


    // OnActivate is called once when the weapon is activated
    protected override void OnActivate()
    {
        if (cooldown <= Time.time)
        {
            cooldown = Time.time + data.cooldown;
            player.controller.StartCoroutine(Dodge());
        }
    }

    public IEnumerator Dodge()
    {

        player.controller.rolling = true;
        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed * data.SpeedMultiplier; //the dodge begins by changing the playerspeed                                                                                        
        yield return new WaitForSeconds(data.moveDuration);

        player.controller.dodgeSpeed = 0f; //the speed is set to 0 to decelarate the player at the end of the dodge
        yield return new WaitForSeconds(data.StopTime);

        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed;
        player.controller.rolling = false;

    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        DodgeRoll copy = new DodgeRoll(weaponData, player);
        return copy;
    }
}
