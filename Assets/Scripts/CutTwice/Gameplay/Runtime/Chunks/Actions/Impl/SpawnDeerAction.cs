using System;
using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Gameplay.Runtime.Chunks.Serialization;
using CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes;
using CutTwice.Gameplay.Runtime.Road.Components;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    [SequenceAction(ActionType.SpawnDeer)]
    public class SpawnDeerAction : ISequenceActionRuntime, IDisposable
    {
        [Serializable]
        public struct Parameters
        {
            public string PrefabKey;
            
            [JsonConverter(typeof(SimplePositionJsonConverter))]
            public SimplePosition Position;
        }
        
        private readonly InfiniteRoadController _infiniteRoadController;
        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly RuntimeLifecycleManager _lifecycleManager;
        
        private GameObject _prefab;
        private Parameters _parameters;

        private CancellationToken _cancellationToken;
        
        public SpawnDeerAction(InfiniteRoadController infiniteRoadController, 
            DeerGameObjectFactory gameObjectFactory, 
            RuntimeLifecycleManager lifecycleManager)
        {
            _infiniteRoadController = infiniteRoadController;
            _gameObjectFactory = gameObjectFactory;
            _lifecycleManager = lifecycleManager;
        }

        async UniTask ISequenceActionRuntime.Init(JObject parameters, CancellationToken ct)
        {
            _parameters = parameters.ToObject<Parameters>();
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

        private void SpawnObject(Transform segment)
        {
            _infiniteRoadController.OnSegmentSpawned.RemoveListener(SpawnObject);
            SpawnObjectAsync(segment, _cancellationToken).Forget();
        }

        private async UniTask SpawnObjectAsync(Transform segment, CancellationToken ct)
        {
            _segment = segment;
            _instance = await _gameObjectFactory.Create(new Vector3(_parameters.Position.X, _parameters.Position.Y, _parameters.Position.Z), _prefab.transform.rotation) as DeerContext;
            await _lifecycleManager.RuntimeRegisterAsync(_instance.RaycastStripController, ct);
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

        public void Dispose()
        {
            DespawnObject(_segment);
        }
    }
}