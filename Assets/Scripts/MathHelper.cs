using UnityEngine;

public static class MathHelper {
    /// <summary>
    /// Interpolates between a and b by t using pow to ease out
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="t">The interpolation value between the two floats.</param>
    /// <param name="p">The value the equation is raised to, causing dampening</param>
    /// <returns></returns>
    public static float LerpDamped(float a, float b, float t, float p) {
        return Mathf.Lerp(a, b, -Mathf.Pow(1 - t, p) + 1);
    }
}