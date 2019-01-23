using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DodgeDash : Weapon
{

    private DodgeData data;
    
    public DodgeDash() : base() { }

    public DodgeDash(WeaponData weaponData, Player player) : base(weaponData, player, Type.Move)
    {
        data = (DodgeData)weaponData;
    }


    // OnActivate is called once when the move is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        player.controller.StartCoroutine(Dodge());
    }

    public IEnumerator Dodge()
    {

        player.controller.dodging = true;
        if(data.MoveDuringDash)
            player.controller.rolling = true;
        // player.controller.ResetCharge();

        /*
        if(data.NewDodge)
        {
            float target = player.stats.moveSpeed * data.SpeedMultiplier;
            player.controller.dodgeSpeed = target; //to start with
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(data.moveDuration / 10);
                player.controller.dodgeSpeed -= (target - player.stats.moveSpeed) * 0.1f;
                //Debug.Log("Speed is now : " + player.controller.dodgeSpeed);
            }
        }
        else
        {
            player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed * data.SpeedMultiplier;
            // player.controller.DisableAttack(data.moveDuration);
            yield return new WaitForSeconds(data.moveDuration);
        }
        */

        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed * data.SpeedMultiplier;
        //player.EnableInvincibility(data.moveDuration / 2);  //the player is Invincible for half of the dash
        yield return new WaitForSeconds(data.moveDuration);
        player.controller.ResetVelocity();


        //player.controller.dodgeSpeed = 0f; //the speed is set to 0 to decelarate the player at the end of the dodge
        
        //player.controller.braking = true;  // this is a different brake implementation, this replaced the dodgeSpeed and ResetVelocity one
        //yield return new WaitForSeconds(data.StopTime);
        //player.controller.braking = false;
        player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed;
        player.controller.dodging = false;
        if (data.MoveDuringDash)
            player.controller.rolling = false;

    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        DodgeDash copy = new DodgeDash(weaponData, player);
        return copy;
    }
}