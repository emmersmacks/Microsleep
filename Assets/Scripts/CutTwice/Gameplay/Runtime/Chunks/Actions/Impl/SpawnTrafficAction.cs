using System;
using System.Threading;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Gameplay.Runtime.Chunks.Serialization;
using CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    [SequenceAction(ActionType.SpawnTraffic)]
    public class SpawnTrafficAction : ISequenceActionRuntime, IDisposable
    {
        [Serializable]
        public struct Parameters
        {
            [JsonConverter(typeof(SimplePositionJsonConverter))]
            public SimplePosition Position;
            
            public SimpleRotation Rotation;
        }

        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly RuntimeLifecycleManager _lifecycleManager;

        private Parameters _parameters;
        private TrafficContext _trafficContext;
        
        public SpawnTrafficAction(TrafficGameObjectFactory gameObjectFactory, RuntimeLifecycleManager lifecycleManager)
        {
            _gameObjectFactory = gameObjectFactory;
            _lifecycleManager = lifecycleManager;
        }
        
        UniTask ISequenceActionRuntime.Init(JObject parameters, CancellationToken ct)
        {
            _parameters = parameters.ToObject<Parameters>();
            return UniTask.CompletedTask;
        }

        async UniTask ISequenceActionRuntime.Run(CancellationToken ct)
        {
            _trafficContext = await _gameObjectFactory.Create(_parameters.Position, _parameters.Rotation) as TrafficContext;
            
            await _lifecycleManager.RuntimeRegisterAsync(_trafficContext.ObjectMoverController, ct);
            await _lifecycleManager.RuntimeRegisterAsync(_trafficContext.RaycastStripController, ct);
            
            _trafficContext.ObjectMoverController.OnFinished += DespawnObject;
        }

        private void DespawnObject()
        {
            _lifecycleManager.Unregister(_trafficContext.ObjectMoverController);
            _lifecycleManager.Unregister(_trafficContext.RaycastStripController);
            Object.Destroy(_trafficContext.GameObject);
            _trafficContext = null;
        }

        public void Dispose()
        {
            _trafficContext.ObjectMoverController.OnFinished -= DespawnObject;
            DespawnObject();
        }
    }
}