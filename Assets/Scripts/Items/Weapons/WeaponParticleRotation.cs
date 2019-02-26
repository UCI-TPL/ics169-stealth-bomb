using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WeaponParticleRotation : MonoBehaviour
{

    public float speed = 10;

    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, speed) * Time.deltaTime;
    }
}
