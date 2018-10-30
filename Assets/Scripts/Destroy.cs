using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

    public float timer;

    private void Start() {
        Destroy(gameObject, timer);
    }
}
