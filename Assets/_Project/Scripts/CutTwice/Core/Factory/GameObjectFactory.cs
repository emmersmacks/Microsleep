using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.Factory
{
    public abstract class GameObjectFactory
    {
        protected GameObject InstantiatePrefab(GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null)
        {
            return Object.Instantiate(
                prefab,
                position,
                rotation,
                parent);
        }

        protected void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}