using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon {

    private LaserWeaponData data;
    private Queue<LaserBeam> laserBeamPool = new Queue<LaserBeam>();
    private LaserBeam CurrentBeam;
    private Buff laserbuff;

    private float startChargeTime;
    protected override float GetChargeLevel() {
        return ((Time.time - startChargeTime) / data.maxChargeTime); // Charge Level is automatically clamped between 0 and 1
    }

    public LaserWeapon() : base() { }

    public LaserWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (LaserWeaponData)weaponData;
    }

    protected override void Start() {
        laserbuff = data.laserBuff.Instance(Mathf.Infinity, this);
    }

    protected override void End() {
        if (CurrentBeam != null) {
            CurrentBeam.TurnOffBeam(null, data.DecayTimePerWidth);
            CurrentBeam = null;
        }
        while (laserBeamPool.Count > 0)
            GameObject.Destroy(laserBeamPool.Dequeue().gameObject);
        player.RemoveBuff(laserbuff);

    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        CurrentBeam = laserBeamPool.Count > 0 ? laserBeamPool.Dequeue() : CreateLaserBeam();
        SetUpLaser(CurrentBeam);
        startChargeTime = Time.time;
        CurrentBeam.gameObject.SetActive(true);
        CurrentBeam.MaxLength = 0;
        CurrentBeam.Width = data.minWidth;
        player.AddBuff(laserbuff);
        GameManager.instance.audioManager.Play("Laser", player.playerNumber);
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        CurrentBeam.MaxLength = Mathf.Lerp(0, data.maxLength, ChargeLevel);
        CurrentBeam.Width = Mathf.Lerp(data.minWidth, data.maxWidth, ChargeLevel);
        CurrentBeam.hitCooldown = Mathf.Lerp(data.hitCooldown / 2, data.hitCooldown, ChargeLevel);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        CurrentBeam.TurnOffBeam(laserBeamPool, data.DecayTimePerWidth);
        CurrentBeam = null;
        player.RemoveBuff(laserbuff);
        GameManager.instance.audioManager.Stop("Laser", player.playerNumber);
    }
    
    private LaserBeam CreateLaserBeam() {
        LaserBeam laserBeam = GameObject.Instantiate<GameObject>(data.LaserBeam.gameObject, player.controller.transform).GetComponent<LaserBeam>();
        laserBeam.OnHit.AddListener((Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target, ChargeLevel); });
        laserBeam.IgnoreCollision = player.controller.gameObject;
        //laserBeam.hitCooldown = data.hitCooldown;
        laserBeam.gameObject.SetActive(false);
        return laserBeam;
    }

    private void SetUpLaser(LaserBeam laserBeam) {
        laserBeam.player = player.controller;
        laserBeam.transform.parent = player.controller.transform;
        laserBeam.transform.forward = player.controller.transform.forward;
        laserBeam.transform.position = player.controller.transform.position + laserBeam.transform.forward * 0.5f;
        laserBeam.SetColor(player.controller.playerColor);
        laserBeam.EnableParticles();
    }

    protected override float GetDamageDealt(Vector3 origin, object extraData) {
        return data.damage * player.stats.Damage * (float)extraData;
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        LaserWeapon copy = new LaserWeapon(weaponData, player);
        return copy;
    }
}
