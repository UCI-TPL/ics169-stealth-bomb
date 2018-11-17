using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DodgeDash : Weapon
{

    private DodgeData data;

    float cooldown = 0.0f;

    public DodgeDash() : base() { }

    public DodgeDash(WeaponData weaponData, Player player) : base(weaponData, player, Type.Move)
    {
        data = (DodgeData)weaponData;
    }


    // OnActivate is called once when the move is activated
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
        player.controller.dodging = true;
        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed * data.SpeedMultiplier;//data.SpeedMultiplier;
        yield return new WaitForSeconds(data.moveDuration);


        player.controller.ResetVelocity();
        player.controller.dodgeSpeed = 0f; //the speed is set to 0 to decelarate the player at the end of the dodge
        
        //player.controller.braking = true;  // this is a different brake implementation, this replaced the dodgeSpeed and ResetVelocity one
        yield return new WaitForSeconds(data.StopTime);
        //player.controller.braking = false;
        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed;
        player.controller.dodging = false;

    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        DodgeDash copy = new DodgeDash(weaponData, player);
        return copy;
    }
}