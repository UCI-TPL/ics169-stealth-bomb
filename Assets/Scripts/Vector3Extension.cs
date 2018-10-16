﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vector3Extensions {
    public static class Vector3Extension {

        // Returns a new Vector3 with the absolute value of the input Vector3
        public static Vector3 Abs(this Vector3 v3) {
            return new Vector3(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
        }

        // Returns a new Vector3Int with all the values of the input Vector3 rounded to the nearest integer
        public static Vector3Int Round(this Vector3 v3) {
            return new Vector3Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y), Mathf.RoundToInt(v3.z));
        }
    }
}