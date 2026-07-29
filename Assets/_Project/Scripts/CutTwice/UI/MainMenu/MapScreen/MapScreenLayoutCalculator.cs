using System.Collections.Generic;
using System.Linq;
using CutTwice.Gameplay.Runtime.Map;
using UnityEngine;

namespace CutTwice.UI.MainMenu.MapScreen
{
    /// <summary>
    /// Runtime BFS-layered layout (ported from the editor-only MapGraphAutoLayout, since GraphView/UI
    /// Toolkit editor APIs aren't available at runtime) restricted to a visible subset of nodes, then
    /// fit-to-screen scaled/centered so the whole visible area always fits inside the container without
    /// panning. Positions are never persisted — recomputed on every screen show against the current
    /// container size.
    /// </summary>
    public static class MapScreenLayoutCalculator
    {
        public static IReadOnlyDictionary<string, Vector2> ComputeLayout(
            MapDefinition map,
            IReadOnlyList<MapNode> visibleNodes,
            Vector2 containerSize,
            float nodeWidth = 200f,
            float nodeHeight = 100f,
            float columnSpacing = 260f,
            float rowSpacing = 140f,
            float padding = 40f)
        {
            var result = new Dictionary<string, Vector2>();
            if (map == null || visibleNodes == null || visibleNodes.Count == 0)
            {
                return result;
            }

            var columns = ComputeFullColumns(map);
            var columnByInstanceId = new Dictionary<string, int>();
            var rowByInstanceId = new Dictionary<string, int>();
            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                var column = columns[columnIndex];
                for (var rowIndex = 0; rowIndex < column.Count; rowIndex++)
                {
                    columnByInstanceId[column[rowIndex]] = columnIndex;
                    rowByInstanceId[column[rowIndex]] = rowIndex;
                }
            }

            var visibleIds = visibleNodes
                .Select(node => node.InstanceId)
                .Where(columnByInstanceId.ContainsKey)
                .ToList();

            if (visibleIds.Count == 0)
            {
                return result;
            }

            // Compact column indices so gaps left by non-visible nodes don't create empty space.
            var usedColumns = visibleIds.Select(id => columnByInstanceId[id]).Distinct().OrderBy(c => c).ToList();
            var compactColumnIndex = usedColumns
                .Select((column, index) => (column, index))
                .ToDictionary(pair => pair.column, pair => pair.index);

            // Compact row indices within each compacted column, preserving original relative order.
            var compactRowIndex = new Dictionary<string, int>();
            foreach (var columnGroup in visibleIds.GroupBy(id => columnByInstanceId[id]))
            {
                var orderedIds = columnGroup.OrderBy(id => rowByInstanceId[id]).ToList();
                for (var rowIndex = 0; rowIndex < orderedIds.Count; rowIndex++)
                {
                    compactRowIndex[orderedIds[rowIndex]] = rowIndex;
                }
            }

            var rawCenters = new Dictionary<string, Vector2>();
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var id in visibleIds)
            {
                var x = compactColumnIndex[columnByInstanceId[id]] * columnSpacing;
                var y = compactRowIndex[id] * rowSpacing;
                var centerX = x + nodeWidth * 0.5f;
                var centerY = y + nodeHeight * 0.5f;
                rawCenters[id] = new Vector2(centerX, centerY);

                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x + nodeWidth);
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y + nodeHeight);
            }

            var boundingWidth = maxX - minX + padding * 2f;
            var boundingHeight = maxY - minY + padding * 2f;

            var scale = 1f;
            if (boundingWidth > 0f && boundingHeight > 0f && containerSize.x > 0f && containerSize.y > 0f)
            {
                scale = Mathf.Min(containerSize.x / boundingWidth, containerSize.y / boundingHeight);
            }

            var rawCenter = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);

            foreach (var id in visibleIds)
            {
                var center = rawCenters[id];
                var offset = center - rawCenter;
                // UI anchoredPosition Y is up-positive; layout rows grow downward, so flip Y.
                result[id] = new Vector2(offset.x * scale, -offset.y * scale);
            }

            return result;
        }

        private static List<List<string>> ComputeFullColumns(MapDefinition map)
        {
            var byId = new Dictionary<string, MapNode>();
            foreach (var node in map.Nodes)
            {
                if (node != null && !string.IsNullOrEmpty(node.InstanceId))
                {
                    byId[node.InstanceId] = node;
                }
            }

            var placed = new HashSet<string>();
            var columns = new List<List<string>>();

            void PlaceComponent(string startId)
            {
                var depth = new Dictionary<string, int> { [startId] = 0 };
                var order = new List<string> { startId };
                var queue = new Queue<string>();
                queue.Enqueue(startId);
                placed.Add(startId);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    var currentDepth = depth[current];

                    foreach (var (_, targetId) in map.GetOutgoing(current))
                    {
                        if (!byId.ContainsKey(targetId) || placed.Contains(targetId))
                        {
                            continue;
                        }

                        depth[targetId] = currentDepth + 1;
                        placed.Add(targetId);
                        order.Add(targetId);
                        queue.Enqueue(targetId);
                    }
                }

                foreach (var id in order)
                {
                    var depthValue = depth[id];
                    while (columns.Count <= depthValue)
                    {
                        columns.Add(new List<string>());
                    }

                    columns[depthValue].Add(id);
                }
            }

            if (!string.IsNullOrEmpty(map.RootInstanceId) && byId.ContainsKey(map.RootInstanceId))
            {
                PlaceComponent(map.RootInstanceId);
            }

            foreach (var id in byId.Keys)
            {
                if (!placed.Contains(id))
                {
                    PlaceComponent(id);
                }
            }

            return columns;
        }
    }
}
