using System;
using System.Threading;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Hazards.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ReflectionObjectPresenter : MonoBehaviour, IInitializable
    {
        [NonSerialized]
        public Material Material;
        
        public UniTask InitAsync(CancellationToken ct)
        {
            Material = GetComponent<MeshRenderer>().material;
            return UniTask.CompletedTask;
        }
    }
}