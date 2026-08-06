using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road.Curves
{
    public static class RoadCurveUtility
    {
        public static Vector3 EvaluatePosition(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
        }

        public static Vector3 EvaluateTangent(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
        {
            float t2 = t * t;

            float h00 = 6f * t2 - 6f * t;
            float h10 = 3f * t2 - 4f * t + 1f;
            float h01 = -6f * t2 + 6f * t;
            float h11 = 3f * t2 - 2f * t;

            return h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
        }
    }
}
