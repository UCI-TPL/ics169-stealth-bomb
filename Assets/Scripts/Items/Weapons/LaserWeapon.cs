﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon {

    private LaserWeaponData data;
    private Queue<LaserBeam> laserBeamPool = new Queue<LaserBeam>();
    private LaserBeam CurrentBeam;
    private float activeID;

    private float startChargeTime;
    protected override float GetChargeLevel() {
        return ((Time.time - startChargeTime) / data.maxChargeTime); // Charge Level is automatically clamped between 0 and 1
    }

    public LaserWeapon() : base() { }

    public LaserWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (LaserWeaponData)weaponData;
    }

    protected override void Start() {
        activeID = 0;
    }

    protected override void End() {
        if (CurrentBeam != null) {
            CurrentBeam.StartCoroutine(TurnOffBeam(CurrentBeam, data.DecayTimePerWidth));
            CurrentBeam = null;
        }
        ++activeID;
        while (laserBeamPool.Count > 0)
            GameObject.Destroy(laserBeamPool.Dequeue().gameObject);
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        CurrentBeam = laserBeamPool.Count > 0 ? laserBeamPool.Dequeue() : CreateLaserBeam();
        SetUpLaser(CurrentBeam);
        startChargeTime = Time.time;
        CurrentBeam.gameObject.SetActive(true);
        CurrentBeam.MaxLength = 0;
        CurrentBeam.Width = data.minWidth;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        CurrentBeam.MaxLength = Mathf.Lerp(0, data.maxLength, ChargeLevel);
        CurrentBeam.Width = Mathf.Lerp(data.minWidth, data.maxWidth, ChargeLevel);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        CurrentBeam.StartCoroutine(TurnOffBeam(CurrentBeam, data.DecayTimePerWidth));
        CurrentBeam = null;
    }

    private IEnumerator TurnOffBeam(LaserBeam laserBeam, float duration) {
        float curID = activeID;
        laserBeam.transform.SetParent(null);
        laserBeam.DisableParticles();
        float currentVelocity = Time.deltaTime / duration;
        while (laserBeam.Width > 0) {
            yield return null;
            laserBeam.Width -= currentVelocity;
        }
        laserBeam.Width = 0;
        laserBeam.gameObject.SetActive(false);
        if (curID != activeID) {
            laserBeamPool.Enqueue(laserBeam);
        }
        else
            GameObject.Destroy(laserBeam.gameObject);
    }
    
    private LaserBeam CreateLaserBeam() {
        LaserBeam laserBeam = GameObject.Instantiate<GameObject>(data.LaserBeam.gameObject, player.controller.transform).GetComponent<LaserBeam>();
        laserBeam.OnHit.AddListener((Vector3 origin, GameObject target) => { Hit(origin, target); });
        laserBeam.IgnoreCollision = player.controller.gameObject;
        laserBeam.hitCooldown = data.hitCooldown;
        laserBeam.gameObject.SetActive(false);
        return laserBeam;
    }

    private void SetUpLaser(LaserBeam laserBeam) {
        laserBeam.transform.parent = player.controller.transform;
        laserBeam.transform.forward = player.controller.transform.forward;
        laserBeam.transform.position = player.controller.transform.position + laserBeam.transform.forward * 0.5f;
        laserBeam.SetColor(player.controller.playerColor);
        laserBeam.EnableParticles();
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        LaserWeapon copy = new LaserWeapon(weaponData, player);
        return copy;
    }
}