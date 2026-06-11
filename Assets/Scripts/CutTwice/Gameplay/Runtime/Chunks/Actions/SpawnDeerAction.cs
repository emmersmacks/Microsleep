using System;
using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.Runtime.Chunks.Serialization;
using CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes;
using CutTwice.Gameplay.Runtime.Road.Components;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    public class SpawnDeerAction : ISequenceActionRuntime
    {
        [Serializable]
        public struct Parameters
        {
            public string PrefabKey;
            
            [JsonConverter(typeof(SimplePositionJsonConverter))]
            public SimplePosition Position;
        }
        
        
        private readonly Parameters _parameters;
        private readonly InfiniteRoadController _infiniteRoadController;
        private readonly IFactoryBase _factory;
        private readonly RuntimeLifecycleManager _lifecycleManager;
        private GameObject _prefab;
        
        private CancellationToken _cancellationToken;

        public SpawnDeerAction(Parameters parameters, InfiniteRoadController infiniteRoadController,
            IFactoryBase factory, RuntimeLifecycleManager lifecycleManager)
        {
            _parameters = parameters;
            _infiniteRoadController = infiniteRoadController;
            _factory = factory;
            _lifecycleManager = lifecycleManager;
        }

        async UniTask ISequenceActionRuntime.Init(CancellationToken ct)
        {
            _prefab = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(_parameters.PrefabKey, ct);
            _cancellationToken = ct;
        }

        UniTask ISequenceActionRuntime.Run(CancellationToken ct)
        {
            _infiniteRoadController.OnSegmentSpawned.AddListener(SpawnObject);
            return UniTask.CompletedTask;
        }

        private DeerContext _instance;
        private Transform _segment;

        private async void SpawnObject(Transform segment)
        {
            _infiniteRoadController.OnSegmentSpawned.RemoveListener(SpawnObject);
            
            _segment = segment;
            _instance = await _factory.Create(new Vector3(_parameters.Position.X, _parameters.Position.Y, _parameters.Position.Z), _prefab.transform.rotation) as DeerContext;
            await _lifecycleManager.RuntimeRegister(_instance.RaycastStripController, _cancellationToken);
            _instance.GameObject.transform.SetParent(_segment);
            
            _infiniteRoadController.OnSegmentSpawned.AddListener(DespawnObject);
        }

        private void DespawnObject(Transform segment)
        {
            if (segment == _segment)
            {
                _infiniteRoadController.OnSegmentSpawned.RemoveListener(DespawnObject);
                
                _lifecycleManager.Unregister(_instance.RaycastStripController);
                Object.Destroy(_instance.GameObject);
                _instance = null;
                _segment = null;
                _cancellationToken = CancellationToken.None;
            }
        }
    }
}