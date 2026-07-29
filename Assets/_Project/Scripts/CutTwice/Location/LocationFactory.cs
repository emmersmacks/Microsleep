using System;
using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.Factory;
using CutTwice.Location.Building;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Location
{
    public class LocationFactory : GameObjectFactory
    {
        public async UniTask<SpotCompositionRoot> Create(string presetId, CancellationToken ct)
        {
            var locationPrefab = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(
                $"Location/{presetId}", ct);

            var instance = InstantiatePrefab(
                locationPrefab,
                locationPrefab.transform.position,
                locationPrefab.transform.rotation);

            var spot = instance.GetComponentInChildren<SpotCompositionRoot>(true);
            if (spot == null)
            {
                throw new InvalidOperationException(
                    $"LocationFactory: instantiated prefab '{locationPrefab.name}' has no SpotCompositionRoot component.");
            }

            return spot;
        }
    }
}
