using CutTwice.Gameplay.Runtime.Road.Components;
using CutTwice.Gameplay.Runtime.Road.Curves;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road
{
    /// <summary>
    /// A node in the branching tree of spawned road segments. Built once at spawn time; only the
    /// tree links (Children) and IsFrozen change afterwards.
    /// </summary>
    public class RoadSegmentNode
    {
        public readonly RoadSegmentPresenter Presenter;
        public readonly RoadSegmentNode Parent;
        public readonly RoadSegmentNode[] Children;
        public readonly RoadSegmentCurve[] ExitCurves;

        /// <summary>Root pose of this node in the shared path-space (presenter-local rest frame).</summary>
        public Vector3 RestPosition;
        public Quaternion RestRotation = Quaternion.identity;

        /// <summary>Absolute arc length (from the very first spawned segment) at which this node's entry point sits. Fixed forever once the node is spawned.</summary>
        public float EntryArcLength;

        public bool IsFrozen;

        public RoadSegmentNode(RoadSegmentPresenter presenter, RoadSegmentNode parent)
        {
            Presenter = presenter;
            Parent = parent;

            int exitCount = presenter.exits?.Length ?? 0;
            Children = new RoadSegmentNode[exitCount];
            ExitCurves = new RoadSegmentCurve[exitCount];

            for (int i = 0; i < exitCount; i++)
            {
                var exitPath = presenter.exits[i];
                if (exitPath == null || exitPath.exitPoint == null || presenter.entryPoint == null)
                    continue;

                ExitCurves[i] = new RoadSegmentCurve(presenter.transform, presenter.entryPoint, exitPath);
            }
        }

        /// <summary>
        /// Curve used for this node's own arc-length bookkeeping and (while it's still the growth
        /// frontier) motion sampling. Reads Presenter.ActiveExitIndex live - safe because a fork node
        /// is never grown past (see InfiniteRoadController.TryGrowActiveTip) until ResolveFork has
        /// already set the index, so nothing ever samples this mid-traversal with a stale choice.
        /// </summary>
        public RoadSegmentCurve ActiveCurve
        {
            get
            {
                int index = Presenter.ActiveExitIndex;
                return (index >= 0 && index < ExitCurves.Length) ? ExitCurves[index] : null;
            }
        }

        public float ActiveExitArcLength => ActiveCurve?.Length ?? 0f;
    }
}
