using System.Collections.Generic;
using System.Linq;

namespace CutTwice.Gameplay.Runtime.Map
{
    public static class MapPathfinder
    {
        /// <summary>
        /// BFS shortest path from fromInstanceId to toInstanceId strictly along map.GetOutgoing
        /// (direction-aware, same traversal rules as MapRuntimeState.AvailableExits /
        /// MapProgressService.IsReachableFromTail). Returns the ordered node list [from, ..., to]
        /// inclusive, or null if toInstanceId is unreachable from fromInstanceId.
        /// </summary>
        public static IReadOnlyList<MapNode> FindShortestPath(MapDefinition map, string fromInstanceId, string toInstanceId)
        {
            var byId = map.Nodes.ToDictionary(node => node.InstanceId);

            if (!byId.ContainsKey(fromInstanceId) || !byId.ContainsKey(toInstanceId))
            {
                return null;
            }

            if (fromInstanceId == toInstanceId)
            {
                return new List<MapNode> { byId[fromInstanceId] };
            }

            var cameFrom = new Dictionary<string, string>();
            var visited = new HashSet<string> { fromInstanceId };
            var queue = new Queue<string>();
            queue.Enqueue(fromInstanceId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var (_, targetId) in map.GetOutgoing(current))
                {
                    if (!visited.Add(targetId))
                    {
                        continue;
                    }

                    cameFrom[targetId] = current;

                    if (targetId == toInstanceId)
                    {
                        return ReconstructPath(byId, cameFrom, fromInstanceId, toInstanceId);
                    }

                    queue.Enqueue(targetId);
                }
            }

            return null;
        }

        private static IReadOnlyList<MapNode> ReconstructPath(
            IReadOnlyDictionary<string, MapNode> byId,
            IReadOnlyDictionary<string, string> cameFrom,
            string fromInstanceId,
            string toInstanceId)
        {
            var idsReversed = new List<string> { toInstanceId };
            var current = toInstanceId;

            while (current != fromInstanceId)
            {
                current = cameFrom[current];
                idsReversed.Add(current);
            }

            idsReversed.Reverse();
            return idsReversed.Select(id => byId[id]).ToList();
        }
    }
}
