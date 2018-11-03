using System.Collections;
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

        // Returns a new Vector3 with all the values of the input Vector3 square rooted
        public static Vector3 Sqrt(this Vector3 v3) {
            return new Vector3(Mathf.Sqrt(v3.x), Mathf.Sqrt(v3.y), Mathf.Sqrt(v3.z));
        }

        // Returns a new Vector3Int with all the values of the input Vector3 rounded to the nearest integer
        public static Vector3 Scaled(this Vector3 v3, Vector3 other) {
            Vector3 result = new Vector3(v3.x, v3.y, v3.z);
            result.Scale(other);
            return result;
        }
    }

    public static class Vector2Extension {

        // Returns a new Vector2 with the absolute value of the input Vector3
        public static Vector2 Abs(this Vector2 v2) {
            return new Vector2(Mathf.Abs(v2.x), Mathf.Abs(v2.y));
        }
    }
}