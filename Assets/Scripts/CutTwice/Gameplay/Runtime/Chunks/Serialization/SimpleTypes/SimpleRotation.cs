using System;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes
{
    [Serializable]
    public struct SimpleRotation
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        
        public static implicit operator Quaternion(SimpleRotation simpleRotation)
        {
            return new Quaternion(simpleRotation.X, simpleRotation.Y, simpleRotation.Z, simpleRotation.W);
        }

        public static implicit operator SimpleRotation(Quaternion quaternion)
        {
            return new SimpleRotation
            {
                X = quaternion.x,
                Y = quaternion.y,
                Z = quaternion.z,
                W = quaternion.w
            };
        }
    }
}