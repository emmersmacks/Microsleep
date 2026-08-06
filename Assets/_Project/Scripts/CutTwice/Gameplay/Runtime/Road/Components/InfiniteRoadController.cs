using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Road;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    /// <summary>
    /// Owns spawning/despawning of the branching road-segment tree. Does not move anything -
    /// segment placement along the road curve is <see cref="InfiniteRoadMotionController"/>'s job.
    /// </summary>
    public class InfiniteRoadController : ITickable, IInitializable
    {
        private const string RoadSegmentPrefabKey = "Road/RoadSegment";

        private readonly InfiniteRoadPresenter _presenter;
        private readonly RuntimeLifecycleManager _lifecycleManager;
        private readonly List<RoadSegmentNode> _liveNodes = new();

        private GameObject _segmentPrefab;
        private GameObject _nextSegment;

        private RoadSegmentNode _activeTip;

        public UnityEvent<RoadSegmentPresenter> OnSegmentSpawned = new();
        public UnityEvent<RoadSegmentPresenter> OnSegmentDespawning = new();

        /// <summary>Monotonic arc length traveled since the road started, never reset.</summary>
        public float TraveledDistance { get; private set; }

        /// <summary>The furthest node on the currently-active branch. Used by the motion controller to walk backwards to any point along the active chain.</summary>
        public RoadSegmentNode ActiveTip => _activeTip;

        /// <summary>Every currently-spawned node (active chain and not-yet-despawned fork branches alike).</summary>
        public IReadOnlyList<RoadSegmentNode> LiveNodes => _liveNodes;

        public InfiniteRoadController(InfiniteRoadPresenter presenter, RuntimeLifecycleManager lifecycleManager)
        {
            _presenter = presenter;
            _lifecycleManager = lifecycleManager;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            _segmentPrefab = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(RoadSegmentPrefabKey, ct);
            _nextSegment = _segmentPrefab;

            var firstPresenter = InstantiateSegment(_nextSegment);
            firstPresenter.transform.localPosition = Vector3.zero;
            firstPresenter.transform.localRotation = Quaternion.identity;

            var root = new RoadSegmentNode(firstPresenter, null)
            {
                RestPosition = Vector3.zero,
                RestRotation = Quaternion.identity,
                EntryArcLength = 0f
            };

            _liveNodes.Add(root);
            _activeTip = root;
            OnSegmentSpawned?.Invoke(firstPresenter);

            FillToDrawDistance();
        }

        public async UniTask SetOneShotSegmentAsync(string segmentKey)
        {
            _nextSegment = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(segmentKey, _presenter.destroyCancellationToken);
            OnSegmentSpawned.AddListener(Handler);

            void Handler(RoadSegmentPresenter segment)
            {
                OnSegmentSpawned.RemoveListener(Handler);
                _nextSegment = _segmentPrefab;
            }
        }

        /// <summary>
        /// Called externally (by CrossroadRouteController) once the player's lane choice at a fork is known.
        /// Until this runs, TryGrowActiveTip refuses to grow past a multi-exit node, so nothing has ever
        /// been spawned/rendered assuming a particular branch - picking one here is a plain handoff, not
        /// a retroactive change to anything already visible.
        /// </summary>
        public void ResolveFork(RoadSegmentPresenter forkSegment, int chosenIndex)
        {
            var forkNode = FindNode(forkSegment);
            if (forkNode == null)
                return;

            forkNode.Presenter.SetActiveExitPoint(chosenIndex);

            for (int i = 0; i < forkNode.Children.Length; i++)
            {
                if (i == chosenIndex)
                    continue;

                var sibling = forkNode.Children[i];
                if (sibling != null)
                    FreezeSubtree(sibling);
            }

            var chosenChild = (chosenIndex >= 0 && chosenIndex < forkNode.Children.Length) ? forkNode.Children[chosenIndex] : null;
            if (chosenChild != null)
                _activeTip = chosenChild;
        }

        public void Tick()
        {
            if (_activeTip == null)
                return;

            TraveledDistance += _presenter.moveSpeed * Time.deltaTime;
            DespawnPassedNodes();
            FillToDrawDistance();
        }

        private void FillToDrawDistance()
        {
            while (ArcLengthAheadOfActiveTip() < _presenter.drawDistance)
            {
                if (!TryGrowActiveTip())
                    break;
            }
        }

        private float ArcLengthAheadOfActiveTip()
        {
            if (_activeTip == null)
                return float.MaxValue;

            return (_activeTip.EntryArcLength + _activeTip.ActiveExitArcLength) - TraveledDistance;
        }

        private bool TryGrowActiveTip()
        {
            var presenter = _activeTip.Presenter;
            int exitCount = presenter.exits?.Length ?? 0;
            if (exitCount == 0)
                return false;

            for (int i = 0; i < exitCount; i++)
            {
                if (_activeTip.Children[i] == null)
                    _activeTip.Children[i] = SpawnChild(_activeTip, i);
            }

            // A fork (multiple exits) spawns a stub into every branch above, but never advances past
            // itself on its own - only ResolveFork (an explicit external choice) may do that. Otherwise
            // the tip would grow down a "default" branch before the choice is known, and later switching
            // branches would mean re-sampling already-traversed curve with a different shape mid-stride.
            if (exitCount > 1)
                return false;

            var onlyChild = _activeTip.Children[0];
            if (onlyChild == null)
                return false;

            _activeTip = onlyChild;
            return true;
        }

        private RoadSegmentNode SpawnChild(RoadSegmentNode parent, int exitIndex)
        {
            var exitPath = parent.Presenter.exits[exitIndex];
            if (exitPath == null || exitPath.exitPoint == null)
                return null;

            var childPresenter = InstantiateSegment(_nextSegment);
            if (childPresenter.entryPoint == null)
            {
                Debug.LogWarning($"RoadSegmentPresenter '{childPresenter.name}' has no entryPoint assigned.", childPresenter);
                Object.Destroy(childPresenter.gameObject);
                return null;
            }

            Transform parentRoot = parent.Presenter.transform;

            // Connector pose relative to the parent's own local hierarchy - independent of wherever
            // the parent's live transform currently sits (it may already be displaced by the motion controller).
            Vector3 connectorLocalPos = parentRoot.InverseTransformPoint(exitPath.exitPoint.position);
            Quaternion connectorLocalRot = Quaternion.Inverse(parentRoot.rotation) * exitPath.exitPoint.rotation;

            Vector3 connectorRestPos = parent.RestPosition + parent.RestRotation * connectorLocalPos;
            Quaternion connectorRestRot = parent.RestRotation * connectorLocalRot;

            Transform childRoot = childPresenter.transform;
            Vector3 entryLocalPos = childRoot.InverseTransformPoint(childPresenter.entryPoint.position);
            Quaternion entryLocalRot = Quaternion.Inverse(childRoot.rotation) * childPresenter.entryPoint.rotation;

            Quaternion childRestRotation = connectorRestRot * Quaternion.Inverse(entryLocalRot);
            Vector3 childRestPosition = connectorRestPos - childRestRotation * entryLocalPos;

            var exitCurve = parent.ExitCurves[exitIndex];
            var node = new RoadSegmentNode(childPresenter, parent)
            {
                RestPosition = childRestPosition,
                RestRotation = childRestRotation,
                EntryArcLength = parent.EntryArcLength + (exitCurve?.Length ?? 0f)
            };

            childRoot.localPosition = childRestPosition;
            childRoot.localRotation = childRestRotation;

            _liveNodes.Add(node);
            OnSegmentSpawned?.Invoke(childPresenter);
            return node;
        }

        private void DespawnPassedNodes()
        {
            float cutoff = TraveledDistance - _presenter.despawnDistance;

            for (int i = _liveNodes.Count - 1; i >= 0; i--)
            {
                var node = _liveNodes[i];
                float endArc = node.EntryArcLength + node.ActiveExitArcLength;
                if (endArc >= cutoff)
                    continue;

                OnSegmentDespawning?.Invoke(node.Presenter);
                _liveNodes.RemoveAt(i);
                DetachFromParent(node);
                Object.Destroy(node.Presenter.gameObject);
            }
        }

        private static void DetachFromParent(RoadSegmentNode node)
        {
            if (node.Parent == null)
                return;

            var siblings = node.Parent.Children;
            for (int i = 0; i < siblings.Length; i++)
            {
                if (siblings[i] == node)
                    siblings[i] = null;
            }
        }

        private static void FreezeSubtree(RoadSegmentNode node)
        {
            node.IsFrozen = true;
            foreach (var child in node.Children)
            {
                if (child != null)
                    FreezeSubtree(child);
            }
        }

        private RoadSegmentNode FindNode(RoadSegmentPresenter presenter)
        {
            foreach (var node in _liveNodes)
            {
                if (node.Presenter == presenter)
                    return node;
            }
            return null;
        }

        private RoadSegmentPresenter InstantiateSegment(GameObject prefab)
        {
            var instance = Object.Instantiate(prefab, _presenter.transform);
            return instance.GetComponent<RoadSegmentPresenter>();
        }
    }
}
