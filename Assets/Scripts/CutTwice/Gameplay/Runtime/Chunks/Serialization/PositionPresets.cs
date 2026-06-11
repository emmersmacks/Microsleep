using System.Collections.Generic;
using CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes;

namespace CutTwice.Gameplay.Runtime.Chunks.Serialization
{
    public static class PositionPresets
    {
        private static readonly Dictionary<string, SimplePosition> _presets = new()
        {
            ["Line1"] = new()
            {
                X = -2.5f,
                Y = 0.0f,
                Z = 70.0f
            },

            ["Line2"] = new()
            {
                X = -5.0f,
                Y = 0.0f,
                Z = 70.0f
            },
            
            ["Line3"] = new()
            {
                X = -7.5f,
                Y = 0.0f,
                Z = 70.0f
            },
            
            ["Line4"] = new()
            {
                X = -10.0f,
                Y = 0.0f,
                Z = 70.0f
            },
            
            ["Line1XOnly"] = new()
            {
                X = -2.5f,
                Y = 0.0f,
                Z = 0.0f
            },

            ["Line2XOnly"] = new()
            {
                X = -5.0f,
                Y = 0.0f,
                Z = 0.0f
            },
            
            ["Line3XOnly"] = new()
            {
                X = -7.5f,
                Y = 0.0f,
                Z = 0.0f
            },
            
            ["Line4XOnly"] = new()
            {
                X = -10.0f,
                Y = 0.0f,
                Z = 0.0f
            },
        };

        public static bool TryGet(string key, out SimplePosition position)
        {
            return _presets.TryGetValue(key, out position);
        }
    }
}