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

        float dSpeed = player.stats.moveSpeed * data.SpeedMultiplier;
        player.controller.dodging = true;
        if(data.MoveDuringDash)
            player.controller.rolling = true;

        float dodgingTime = Time.time + data.moveDuration;
        player.controller.dodgeSpeed = player.stats.moveSpeed * data.SpeedMultiplier;
        while(dodgingTime >= Time.time)
        {
            //Debug.Log("Frame time is " + Time.deltaTime);
            //Time.deltaTime is 0.09 for 11fps and 0.05 for 130fps
            //the speed should decrease MORE at lower FPS to make up for the lack of decrements. At 11fps it might only decrease twice while at 100+ fps the while loop might run way more! 
            float decrease =  dSpeed * data.DodgeDeceleration * Time.deltaTime; //if DodgeDeceleration isn't 0 the dodge will slow down as the dodge goes on, hopefully looking smooth
            float percentage = (decrease * 100) / dSpeed;
            float percentageChange = (dSpeed * 100) / (player.stats.moveSpeed * data.SpeedMultiplier);
            dSpeed -= decrease;
            player.controller.dodgeSpeed = dSpeed;

            //Debug.Log(" percentage of change : "+percentage+" percentage changed is "+percentageChange);
            //yield return null;
            yield return new WaitForEndOfFrame();
        }
        player.controller.dodgeSpeed = player.stats.moveSpeed;
        player.controller.ResetVelocity();

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

        //player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed * data.SpeedMultiplier;
        //player.EnableInvincibility(data.moveDuration / 2);  //the player is Invincible for half of the dash
        //yield return new WaitForSeconds(data.moveDuration);
        //player.controller.ResetVelocity();
        //player.controller.dodgeSpeed = player.controller.player.stats.moveSpeed;

        player.controller.dodging = false;
        if (data.MoveDuringDash)
            player.controller.rolling = false;
       // yield return null;

    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        DodgeDash copy = new DodgeDash(weaponData, player);
        return copy;
    }
}