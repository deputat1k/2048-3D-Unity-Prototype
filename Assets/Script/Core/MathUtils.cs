using UnityEngine;

namespace Cube2048.Core
{
    public static class MathUtils
    {
        // Ми прибрали FindBestPair, бо він тепер у Стратегіях.
        // Але залишили чисту математику для кривих (Bezier Curve).

        public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            // Формула квадратичної кривої Безьє
            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }
    }
}