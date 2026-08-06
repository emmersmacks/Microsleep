using System;
using CutTwice.Gameplay.Runtime.Road.Components;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road.Curves
{
    /// <summary>
    /// Arc-length parameterized Hermite curve through a segment's entry point, its exit path's
    /// control points and the exit point, all expressed in the segment root's local space.
    /// Edge tangents are forced to the entry/exit transforms' forward direction so that curves
    /// of consecutive chained segments stay C1-continuous at the connector.
    /// </summary>
    public class RoadSegmentCurve
    {
        private const int SamplesPerSpan = 16;

        private readonly float[] _cumulativeLength;
        private readonly Vector3[] _positions;
        private readonly Quaternion[] _rotations;

        public float Length { get; }

        public RoadSegmentCurve(Transform segmentRoot, Transform entryPoint, RoadExitPath exitPath)
        {
            var controlPoints = exitPath.controlPoints ?? Array.Empty<Transform>();
            int knotCount = 2 + controlPoints.Length;

            var knotPos = new Vector3[knotCount];
            var knotUp = new Vector3[knotCount];

            knotPos[0] = segmentRoot.InverseTransformPoint(entryPoint.position);
            knotUp[0] = segmentRoot.InverseTransformDirection(entryPoint.up);

            for (int i = 0; i < controlPoints.Length; i++)
            {
                knotPos[i + 1] = segmentRoot.InverseTransformPoint(controlPoints[i].position);
                knotUp[i + 1] = segmentRoot.InverseTransformDirection(controlPoints[i].up);
            }

            int last = knotCount - 1;
            knotPos[last] = segmentRoot.InverseTransformPoint(exitPath.exitPoint.position);
            knotUp[last] = segmentRoot.InverseTransformDirection(exitPath.exitPoint.up);

            var tangents = new Vector3[knotCount];
            Vector3 entryDir = segmentRoot.InverseTransformDirection(entryPoint.forward).normalized;
            Vector3 exitDir = segmentRoot.InverseTransformDirection(exitPath.exitPoint.forward).normalized;

            tangents[0] = entryDir * (knotPos[1] - knotPos[0]).magnitude;
            tangents[last] = exitDir * (knotPos[last] - knotPos[last - 1]).magnitude;
            for (int i = 1; i < last; i++)
                tangents[i] = 0.5f * (knotPos[i + 1] - knotPos[i - 1]);

            int numSpans = knotCount - 1;
            int sampleCount = numSpans * SamplesPerSpan + 1;
            _cumulativeLength = new float[sampleCount];
            _positions = new Vector3[sampleCount];
            _rotations = new Quaternion[sampleCount];

            int index = 0;
            Vector3 prevPos = knotPos[0];
            float cumulative = 0f;

            _positions[index] = prevPos;
            _rotations[index] = LookRotationSafe(entryDir, knotUp[0]);
            _cumulativeLength[index] = 0f;
            index++;

            for (int span = 0; span < numSpans; span++)
            {
                Vector3 p0 = knotPos[span];
                Vector3 m0 = tangents[span];
                Vector3 p1 = knotPos[span + 1];
                Vector3 m1 = tangents[span + 1];
                Vector3 up0 = knotUp[span];
                Vector3 up1 = knotUp[span + 1];

                for (int step = 1; step <= SamplesPerSpan; step++)
                {
                    float t = step / (float)SamplesPerSpan;
                    Vector3 pos = RoadCurveUtility.EvaluatePosition(p0, m0, p1, m1, t);
                    Vector3 tangent = RoadCurveUtility.EvaluateTangent(p0, m0, p1, m1, t);
                    Vector3 up = Vector3.Slerp(up0, up1, t);

                    cumulative += Vector3.Distance(prevPos, pos);
                    prevPos = pos;

                    _positions[index] = pos;
                    _rotations[index] = LookRotationSafe(tangent, up);
                    _cumulativeLength[index] = cumulative;
                    index++;
                }
            }

            Length = cumulative;
        }

        public (Vector3 position, Quaternion rotation) SampleByArcLength(float distance)
        {
            distance = Mathf.Clamp(distance, 0f, Length);

            int lo = 0;
            int hi = _cumulativeLength.Length - 1;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (_cumulativeLength[mid] < distance)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            if (lo == 0)
                return (_positions[0], _rotations[0]);

            float segLength = _cumulativeLength[lo] - _cumulativeLength[lo - 1];
            float localT = segLength > 1e-6f ? (distance - _cumulativeLength[lo - 1]) / segLength : 0f;

            Vector3 pos = Vector3.Lerp(_positions[lo - 1], _positions[lo], localT);
            Quaternion rot = Quaternion.Slerp(_rotations[lo - 1], _rotations[lo], localT);
            return (pos, rot);
        }

        private static Quaternion LookRotationSafe(Vector3 forward, Vector3 up)
        {
            if (forward.sqrMagnitude < 1e-8f)
                forward = Vector3.forward;
            if (up.sqrMagnitude < 1e-8f)
                up = Vector3.up;
            return Quaternion.LookRotation(forward, up);
        }
    }
}
