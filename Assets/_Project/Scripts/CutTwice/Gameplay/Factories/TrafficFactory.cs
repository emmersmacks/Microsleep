using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Obstacles.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Factories
{
    public class TrafficContext : Context
    {
        public ObjectMoverController ObjectMoverController;
        public RaycastStripController RaycastStripController;
    }
    
    public class TrafficGameObjectFactory : ActionObjectFactory  {
        private readonly Core.EventBus.IEventBus _eventBus;

        public TrafficGameObjectFactory(Core.EventBus.IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        protected override string PrefabKey => "Obstacles/Traffic";

        public override UniTask<Context> Create(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var trafficContext = new TrafficContext();
            
            trafficContext.GameObject = InstantiatePrefab(_prefab, position, rotation, parent);
            
            var objectMoverPresenter = trafficContext.GameObject.GetComponent<ObjectMoverPresenter>();
            trafficContext.ObjectMoverController = new ObjectMoverController(objectMoverPresenter);
            
            var raycastStripPresenter = trafficContext.GameObject.GetComponent<RaycastStripPresenter>();
            trafficContext.RaycastStripController = new RaycastStripController(raycastStripPresenter, () => _eventBus.Publish(new Core.EventBus.GameOverEvent()));
            
            return UniTask.FromResult((Context)trafficContext);
        }
    }
}