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

        GameManager.instance.audioManager.Play("Dash");

        while (dodgingTime >= Time.time)
        {
            //Time.deltaTime is 0.09 for 11fps and 0.05 for 130fps
            //the speed should decrease MORE at lower FPS to make up for the lack of decrements. At 11fps it might only decrease twice while at 100+ fps the while loop might run way more! 
            float decrease =  dSpeed * data.DodgeDeceleration * Time.deltaTime; //if DodgeDeceleration isn't 0 the dodge will slow down as the dodge goes on, hopefully looking smooth
            float percentage = (decrease * 100) / dSpeed;
            float percentageChange = (dSpeed * 100) / (player.stats.moveSpeed * data.SpeedMultiplier);
            dSpeed -= decrease;
            player.controller.dodgeSpeed = dSpeed;
            yield return new WaitForEndOfFrame();
        }
        player.controller.dodgeSpeed = player.stats.moveSpeed;
        player.controller.ResetVelocity();



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