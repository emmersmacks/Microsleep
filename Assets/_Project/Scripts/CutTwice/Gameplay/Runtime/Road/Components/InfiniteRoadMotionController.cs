using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Road;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    /// <summary>
    /// Every frame, repositions the whole spawned road-segment tree as one rigid body so that the
    /// composite curve, sampled at the controller's current traveled distance, lands exactly on the
    /// presenter's origin. Because every live node shares the same per-frame rigid transform, the
    /// entire visible strip smoothly rotates together as a bend in the curve approaches/passes through
    /// the anchor - the "road rides on rails" effect. Must tick after <see cref="InfiniteRoadController"/>.
    /// </summary>
    public class InfiniteRoadMotionController : ITickable
    {
        private readonly InfiniteRoadController _roadController;

        public InfiniteRoadMotionController(InfiniteRoadController roadController)
        {
            _roadController = roadController;
        }

        public void Tick()
        {
            var activeTip = _roadController.ActiveTip;
            if (activeTip == null)
                return;

            float distance = _roadController.TraveledDistance;
            var node = FindNodeContaining(activeTip, distance);
            var curve = node.ActiveCurve;
            if (curve == null)
                return;

            float localDistance = distance - node.EntryArcLength;
            var (sampleLocalPos, sampleLocalRot) = curve.SampleByArcLength(localDistance);

            Vector3 samplePos = node.RestPosition + node.RestRotation * sampleLocalPos;
            Quaternion sampleRot = node.RestRotation * sampleLocalRot;

            Quaternion deltaRotation = Quaternion.Inverse(sampleRot);
            Vector3 deltaPosition = -(deltaRotation * samplePos);

            var liveNodes = _roadController.LiveNodes;
            for (int i = 0; i < liveNodes.Count; i++)
            {
                var liveNode = liveNodes[i];
                if (liveNode.IsFrozen)
                    continue;

                var root = liveNode.Presenter.transform;
                root.localPosition = deltaPosition + deltaRotation * liveNode.RestPosition;
                root.localRotation = deltaRotation * liveNode.RestRotation;
            }
        }

        private static RoadSegmentNode FindNodeContaining(RoadSegmentNode fromTip, float distance)
        {
            var node = fromTip;
            while (node.Parent != null && distance < node.EntryArcLength)
            {
                node = node.Parent;
            }
            return node;
        }
    }
}
