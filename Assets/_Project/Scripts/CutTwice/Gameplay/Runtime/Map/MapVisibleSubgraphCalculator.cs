using System.Collections.Generic;
using System.Linq;

namespace CutTwice.Gameplay.Runtime.Map
{
    public readonly struct MapVisibleSubgraph
    {
        public IReadOnlyList<MapNode> Nodes { get; }
        public IReadOnlyList<MapConnection> Connections { get; }

        public MapVisibleSubgraph(IReadOnlyList<MapNode> nodes, IReadOnlyList<MapConnection> connections)
        {
            Nodes = nodes;
            Connections = connections;
        }
    }

    /// <summary>
    /// Computes the subset of a map's graph that should be drawn on the map screen: every node
    /// ever visited (state.History) plus everything reachable within `radius` forward hops from
    /// CurrentNode. Visibility is derived from the same MapDefinition.GetOutgoing traversal used
    /// by MapPathfinder/MapProgressService, so anything visible is guaranteed to be a valid route target.
    /// </summary>
    public static class MapVisibleSubgraphCalculator
    {
        public static MapVisibleSubgraph Compute(MapRuntimeState state, int radius = 1)
        {
            var byId = state.Map.Nodes.ToDictionary(node => node.InstanceId);

            var visibleIds = new HashSet<string>();
            foreach (var node in state.History)
            {
                visibleIds.Add(node.InstanceId);
            }

            var frontier = new List<string> { state.CurrentNode.InstanceId };
            visibleIds.Add(state.CurrentNode.InstanceId);

            for (var hop = 0; hop < radius && frontier.Count > 0; hop++)
            {
                var nextFrontier = new List<string>();
                foreach (var current in frontier)
                {
                    foreach (var (_, targetId) in state.Map.GetOutgoing(current))
                    {
                        if (visibleIds.Add(targetId))
                        {
                            nextFrontier.Add(targetId);
                        }
                    }
                }

                frontier = nextFrontier;
            }

            var visibleNodes = visibleIds
                .Where(byId.ContainsKey)
                .Select(id => byId[id])
                .ToList();

            var visibleConnections = state.Map.Connections
                .Where(connection => visibleIds.Contains(connection.NodeAId) && visibleIds.Contains(connection.NodeBId))
                .ToList();

            return new MapVisibleSubgraph(visibleNodes, visibleConnections);
        }
    }
}
