using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Obstacles;
using CutTwice.Gameplay.Runtime.Obstacles.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Factories
{
    public class DeerContext : Context
    {
        public RaycastStripController RaycastStripController;
    }
    
    public class DeerFactory : GameObjectFactory
    {
        protected override string PrefabKey => "Obstacles/Deer";
        private readonly CutTwice.Core.EventBus.IEventBus _eventBus;

        public DeerFactory(CutTwice.Core.EventBus.IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override UniTask<Context> Create(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var deerContext = new DeerContext();
            
            deerContext.GameObject = InstantiatePrefab(_prefab, position, rotation, parent);
            
            var raycastStripPresenter = deerContext.GameObject.GetComponent<RaycastStripPresenter>();
            deerContext.RaycastStripController = new RaycastStripController(raycastStripPresenter, _eventBus);
            
            return UniTask.FromResult((Context)deerContext);
        }
    }
}