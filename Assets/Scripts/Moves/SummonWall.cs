using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SummonWall : Weapon {

    public SummonData data;

    float cooldown = 0;

    public SummonWall() : base() { }

    public SummonWall(WeaponData weaponData, Player player) : base(weaponData, player, Type.Move)
    {
        data = (SummonData)weaponData;
    }


    protected override void OnActivate()
    {
        if (cooldown <= Time.time)
        {
            cooldown = Time.time + data.cooldown;
            player.controller.StartCoroutine(Summon());
        }
    }

    public IEnumerator Summon()
    {
        Vector3 spawnPosition = player.controller.transform.position + (player.controller.transform.forward *  data.distanceFromPlayer);
        GameObject summon = GameObject.Instantiate(data.summon);
        //GameObject summon = player.controller.InstantiateSummon(data.summon);
        summon.transform.position = spawnPosition;
        summon.transform.rotation = player.controller.transform.rotation;
        yield return new WaitForSeconds(data.moveDuration);
        GameObject.Destroy(summon);
        //player.controller.DestroySummon(summon);
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player)
    {
        DodgeDash copy = new DodgeDash(weaponData, player);
        return copy;
    }
}
