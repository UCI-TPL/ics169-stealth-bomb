using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorExtensions {
    public static class ColorExtension {
        // Returns a new Vector3 with the absolute value of the input Vector3
        public static Color ScaleHSV(this Color c, Color scale, bool hdr = false, bool multiplyBaseValue = true) {
            Color.RGBToHSV(c, out float targetH, out float targetS, out float targetV);
            Color.RGBToHSV(scale, out float H, out float S, out float V);
            Color result = Color.HSVToRGB(targetH, S * targetS, V * (multiplyBaseValue ? targetV : 1), hdr);
            result.a = scale.a;
            return result;
        }
    }
}
