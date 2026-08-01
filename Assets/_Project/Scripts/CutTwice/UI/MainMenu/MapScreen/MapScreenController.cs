using System;
using System.Collections.Generic;
using System.Linq;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Runtime.Map;
using UnityEngine;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapScreenController : WindowControllerBase<MapWindowView>, IDisposable
    {
        private const int VisibilityRadius = 1;

        private readonly MapProgressService _mapProgressService;
        private readonly Dictionary<string, MapNodeView> _nodeViewsByInstanceId = new();
        private readonly Dictionary<MapConnection, MapConnectionView> _connectionViewsByConnection = new();

        public MapScreenController(MapWindowView view, MapProgressService mapProgressService) : base(view)
        {
            _mapProgressService = mapProgressService;
        }

        protected override void OnShow(object payload)
        {
            Rebuild();
        }

        private void Rebuild()
        {
            DestroyExistingViews();

            var state = _mapProgressService.GetRuntimeState();
            var visible = MapVisibleSubgraphCalculator.Compute(state, VisibilityRadius);
            var positions = MapScreenLayoutCalculator.ComputeLayout(state.Map, visible.Nodes, View.ContentRoot.rect.size);

            // Connections are spawned before nodes so their sibling index is lower — uGUI renders
            // later siblings on top, which keeps connection lines drawn behind the node views.
            foreach (var connection in visible.Connections)
            {
                if (!positions.TryGetValue(connection.NodeAId, out var from) ||
                    !positions.TryGetValue(connection.NodeBId, out var to))
                {
                    continue;
                }

                var connectionView = UnityEngine.Object.Instantiate(View.ConnectionViewPrefab, View.ContentRoot);
                connectionView.SetEndpoints(from, to);
                _connectionViewsByConnection[connection] = connectionView;
            }

            var visitedIds = new HashSet<string>(state.History.Select(n => n.InstanceId));

            foreach (var node in visible.Nodes)
            {
                var nodeView = UnityEngine.Object.Instantiate(View.NodeViewPrefab, View.ContentRoot);
                nodeView.Bind(node);
                var isCompleted = node.InstanceId != state.CurrentNode.InstanceId && visitedIds.Contains(node.InstanceId);
                nodeView.SetCompleted(isCompleted);

                if (positions.TryGetValue(node.InstanceId, out var position))
                {
                    ((RectTransform)nodeView.transform).anchoredPosition = position;
                }

                nodeView.Clicked += OnNodeClicked;
                _nodeViewsByInstanceId[node.InstanceId] = nodeView;
            }

            HighlightRoute(state.CurrentNode, _mapProgressService.GetQueuedRoute());
        }

        private void OnNodeClicked(MapNodeView nodeView)
        {
            var state = _mapProgressService.GetRuntimeState();
            if (nodeView.Node.InstanceId == state.CurrentNode.InstanceId)
            {
                return;
            }

            var path = MapPathfinder.FindShortestPath(state.Map, state.CurrentNode.InstanceId, nodeView.Node.InstanceId);
            if (path == null || path.Count < 2)
            {
                return;
            }

            _mapProgressService.ClearRoute();
            foreach (var node in path.Skip(1))
            {
                _mapProgressService.AddTargetLocation(node);
            }

            HighlightRoute(state.CurrentNode, _mapProgressService.GetQueuedRoute());
        }

        private void HighlightRoute(MapNode currentNode, IReadOnlyList<MapNode> queuedRoute)
        {
            foreach (var nodeView in _nodeViewsByInstanceId.Values)
            {
                nodeView.SetHighlighted(false);
            }

            foreach (var connectionView in _connectionViewsByConnection.Values)
            {
                connectionView.SetHighlighted(false);
            }

            var sequence = new List<MapNode> { currentNode };
            sequence.AddRange(queuedRoute);

            foreach (var node in sequence)
            {
                if (_nodeViewsByInstanceId.TryGetValue(node.InstanceId, out var nodeView))
                {
                    nodeView.SetHighlighted(true);
                }
            }

            for (var i = 0; i < sequence.Count - 1; i++)
            {
                var aId = sequence[i].InstanceId;
                var bId = sequence[i + 1].InstanceId;

                var connection = _connectionViewsByConnection.Keys.FirstOrDefault(c =>
                    (c.NodeAId == aId && c.NodeBId == bId) || (c.NodeAId == bId && c.NodeBId == aId));

                if (connection != null && _connectionViewsByConnection.TryGetValue(connection, out var connectionView))
                {
                    connectionView.SetHighlighted(true);
                }
            }
        }

        private void DestroyExistingViews()
        {
            foreach (var nodeView in _nodeViewsByInstanceId.Values)
            {
                nodeView.Clicked -= OnNodeClicked;
                if (nodeView != null)
                {
                    UnityEngine.Object.Destroy(nodeView.gameObject);
                }
            }

            _nodeViewsByInstanceId.Clear();

            foreach (var connectionView in _connectionViewsByConnection.Values)
            {
                if (connectionView != null)
                {
                    UnityEngine.Object.Destroy(connectionView.gameObject);
                }
            }

            _connectionViewsByConnection.Clear();
        }

        public void Dispose()
        {
            DestroyExistingViews();
        }
    }
}
