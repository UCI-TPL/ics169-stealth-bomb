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

        float target = player.stats.moveSpeed * data.SpeedMultiplier;
        player.controller.dodgeSpeed = target;// player.controller.player.stats.moveSpeed * data.SpeedMultiplier;
        //player.EnableInvincibility(data.moveDuration / 2);  //the player is Invincible for half of the dash

        float movingTime = Time.time + data.moveDuration;
        float timeDecrease  = 15f; //3f is good for deltaTime oh my

        float decreaseLimit = 0.7f; //stops the dodgeSpeed from growing too low at high framerates
        while(movingTime >= Time.time)
        {
            Debug.Log("Speed is " + player.controller.dodgeSpeed);
            if(data.NewDodge) //ignore this for now it isn't currently in use
            {
                /*
                yield return new WaitForSeconds(data.moveDuration / 10);
                Debug.Log(" t os " + target + " and mov speed is " + player.stats.moveSpeed);
                Debug.Log("Minus by "+ (target - player.stats.moveSpeed) * 0.1f * timeMultiplier);
                float subtraction = (target - player.stats.moveSpeed) * 0.1f * timeMultiplier;
                if(-subtraction < player.controller.dodgeSpeed)
                    player.controller.dodgeSpeed += (target - player.stats.moveSpeed) * 0.1f * timeMultiplier;
                timeMultiplier += 0.5f;
                */
                float subtraction = player.controller.dodgeSpeed * timeDecrease * Time.fixedDeltaTime;
                player.controller.dodgeSpeed = player.controller.dodgeSpeed - subtraction;

                if(player.controller.dodgeSpeed <= (target * decreaseLimit)) //this is to prevent the speed for dipping too low at high FPS. Out game is often in the 100s
                {
                    player.controller.dodgeSpeed = target * (decreaseLimit + 0.15f);
                }
                yield return null;
            }
            else
            {
                yield return null; //the crucial line that should never be removed.
            }
            
        }
        //yield return new WaitForSeconds(data.moveDuration);
        player.controller.ResetVelocity();
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