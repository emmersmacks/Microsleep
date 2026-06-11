using System;
using System.Threading;
using CutTwice.App;
using CutTwice.Core.Addressables;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.Initialization
{
    public abstract class Bootstrap : MonoBehaviour
    {
        public bool InstantiateAppBootstrap;
        
        protected CompositionRoot CompositionRoot;
        protected RuntimeLifecycleManager LifecycleManager;
        
        protected abstract CompositionRoot CreateCompositionRoot();
        
        private void Awake()
        {
            AwakeAsync().Forget(Debug.LogException);
        }

        private async UniTask AwakeAsync()
        {
            if (InstantiateAppBootstrap && FindAnyObjectByType<AppBootstrap>() == null)
            {
                var bootstrap = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>("AppBootstrap", destroyCancellationToken);
                Instantiate(bootstrap);
            }
            CompositionRoot = CreateCompositionRoot();

            LifecycleManager = FindAnyObjectByType<RuntimeLifecycleManager>();
            if (LifecycleManager == null)
            {
                var lifecycleManagerObj = new GameObject("LifeCycleManager");
                LifecycleManager = lifecycleManagerObj.AddComponent<RuntimeLifecycleManager>();
            }
            
            
            CompositionRoot.Compose(LifecycleManager);
            
            await LifecycleManager.InitAsync(destroyCancellationToken);
            await InitAsync(destroyCancellationToken);
        }

        protected virtual UniTask InitAsync(CancellationToken ct) { return UniTask.CompletedTask; }

        private void OnDestroy()
        {
            CompositionRoot = null;
            LifecycleManager = null;
        }
    }
}