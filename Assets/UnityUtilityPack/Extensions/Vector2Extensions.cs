using UnityEngine;

namespace UnityUtils
{
    public static class Vector2Extensions
    {
        public static float DistanceTo(this Vector2 from, Vector2 to)
        {
            return Vector2.Distance(from, to);
        }
        
        public static Vector2 RotateTowards(this Vector2 from, Vector2 to, float maxDegrees)
        {
            if (from == Vector2.zero || to == Vector2.zero)
                return Vector2.Lerp(from, to, maxDegrees / 180f);

            float maxRadians = Mathf.Deg2Rad * maxDegrees;
            float angle = Vector2.Angle(from, to) * Mathf.Deg2Rad;
            float t = Mathf.Clamp01(maxRadians / Mathf.Max(angle, Mathf.Epsilon));
            Vector2 fromNorm = from.normalized;
            Vector2 toNorm = to.normalized;
            Vector2 result = Vector2.Lerp(fromNorm, toNorm, t);
            return result.normalized * Mathf.Lerp(from.magnitude, to.magnitude, t);
        }
        
        public static Vector2 LerpRotateTowards(this Vector2 from, Vector2 to, float t)
        {
            if (from == Vector2.zero || to == Vector2.zero)
                return Vector2.Lerp(from, to, t);

            Vector2 fromNorm = from.normalized;
            Vector2 toNorm = to.normalized;
            Vector2 result = Vector2.Lerp(fromNorm, toNorm, t);
            return result.normalized * Mathf.Lerp(from.magnitude, to.magnitude, t);
        }
    }
}