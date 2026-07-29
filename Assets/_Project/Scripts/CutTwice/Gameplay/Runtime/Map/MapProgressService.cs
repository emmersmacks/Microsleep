using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Map
{
    public class MapProgressService
    {
        private readonly Queue<MapNode> _routeQueue = new();

        private MapDefinition _selectedMap;
        private MapRuntimeState _runtimeState;

        public bool HasSelectedMap => _selectedMap != null;
        public bool IsMapActive => _runtimeState != null;

        public void SelectMap(MapDefinition map)
        {
            _selectedMap = map;
        }

        public void ActivateSelectedMap()
        {
            if (_selectedMap == null)
            {
                throw new InvalidOperationException("MapProgressService: no map selected.");
            }

            if (_runtimeState != null)
            {
                return;
            }

            _runtimeState = new MapRuntimeState(_selectedMap);
            Debug.Log($"[MAPS]: Activated map '{_selectedMap.name}'");
        }

        public MapNode GetLastVisitedLocation()
        {
            return RequireActiveState().CurrentNode;
        }

        public MapNode GetNextTargetLocation()
        {
            var state = RequireActiveState();
            if (_routeQueue.Count > 0)
            {
                return _routeQueue.Peek();
            }

            var exits = state.AvailableExits;
            return exits.Count == 1 ? exits[0].Target : null;
        }

        public void AddTargetLocation(MapNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            RequireActiveState();

            if (!IsReachableFromTail(node))
            {
                throw new InvalidOperationException(
                    $"MapProgressService: '{node.InstanceId}' is not a valid outgoing exit from the current route position.");
            }

            _routeQueue.Enqueue(node);
        }

        public void MarkLocationCompleted()
        {
            var state = RequireActiveState();
            var target = GetNextTargetLocation();
            
            if (target == null)
            {
                throw new InvalidOperationException(
                    $"MapProgressService: cannot complete — no target queued and current node '{state.CurrentNode.InstanceId}' has {state.AvailableExits.Count} exits.");
            }

            if (_routeQueue.Count > 0)
            {
                _routeQueue.Dequeue();
            }

            var transition = state.AvailableExits.First(t => t.Target.InstanceId == target.InstanceId);
            state.MoveTo(transition);
            
            Debug.Log($"[MAPS]: MarkLocationCompleted '{state.Map.DisplayName}' as {state.CurrentNode}");
        }

        public bool IsCurrentPositionDeadEnd()
        {
            return RequireActiveState().AvailableExits.Count == 0;
        }

        public IReadOnlyList<MapNode> GetQueuedRoute()
        {
            return _routeQueue.ToArray();
        }

        public void ClearRoute()
        {
            _routeQueue.Clear();
        }

        public MapRuntimeState GetRuntimeState()
        {
            return RequireActiveState();
        }

        public void ResetProgress()
        {
            _runtimeState = null;
            _routeQueue.Clear();
        }

        private MapRuntimeState RequireActiveState()
        {
            if (_runtimeState == null)
            {
                throw new InvalidOperationException("MapProgressService: no active map traversal.");
            }

            return _runtimeState;
        }

        private bool IsReachableFromTail(MapNode candidate)
        {
            var tailId = _routeQueue.Count > 0 ? _routeQueue.Last().InstanceId : _runtimeState.CurrentNode.InstanceId;
            return _selectedMap.GetOutgoing(tailId).Any(exit => exit.TargetId == candidate.InstanceId);
        }
    }
}
