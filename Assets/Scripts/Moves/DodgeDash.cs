using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeDash : SpecialMove
{
    public DodgeData data;
    float cooldown = 0.0f;

    /* not currently used 
    public DodgeDash(DodgeData data, PlayerController controller) : base(controller)
    {
        this.data = data;
        this.controller = controller;
    }
    */

    public override void SetData(PlayerController controller, SpecialMoveData data) //replacement constructor
    {
        this.controller = controller;
        this.data = (DodgeData)data;
    }

    public override void Activate()
    {
        if (cooldown <= Time.time)
        {
            cooldown = Time.time + data.cooldown;
            StartCoroutine(Dodge());
        }
    }

    public IEnumerator Dodge()
    {
        controller.dodging = true;
        controller.dodgeSpeed = controller.player.stats.moveSpeed * data.SpeedMultiplier;//data.SpeedMultiplier;

        yield return new WaitForSeconds(data.moveDuration);
        controller.dodgeSpeed = 0f; //the speed is set to 0 to decelarate the player at the end of the dodge

        yield return new WaitForSeconds(data.moveDuration/2);
        controller.dodgeSpeed = controller.player.stats.moveSpeed;
        controller.dodging = false;
    }

}
