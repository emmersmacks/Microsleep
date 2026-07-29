using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.Factory
{
    public abstract class ActionObjectFactory : GameObjectFactory, IInitializable
    {
        protected abstract string PrefabKey { get; }
        
        protected GameObject _prefab;

        public async UniTask InitAsync(CancellationToken ct)
        {
            _prefab = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(PrefabKey, ct);
        }

        public abstract UniTask<Context> Create(
            Vector3 position,
            Quaternion rotation,
            Transform parent = null);
    }
}