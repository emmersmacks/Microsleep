using System;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes
{
    [Serializable]
    public struct SimplePosition
    {
        public float X;
        public float Y;
        public float Z;
        
        public static implicit operator Vector3(SimplePosition simplePosition)
        {
            return new Vector3(simplePosition.X, simplePosition.Y, simplePosition.Z);
        }

        public static implicit operator SimplePosition(Vector3 vector)
        {
            return new SimplePosition
            {
                X = vector.x,
                Y = vector.y,
                Z = vector.z
            };
        }
    }
}