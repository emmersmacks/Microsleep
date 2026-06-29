using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.Lifecycle
{
    public class RuntimeLifecycleManager : MonoBehaviour
    {
        private List<IInitializable> _initializableList = new ();
        private List<ITickable> _tickableList = new ();
        private List<ILateTickable> _lateTickableList = new ();
        private List<IFixedTickable> _fixedTickableList = new ();
        private List<IDisposable> _disposableList = new ();

        private readonly List<object> _pendingUnregister = new();

        public async UniTask InitAsync(CancellationToken ct)
        {
            foreach (var initializable in _initializableList)
            {
                await initializable.InitAsync(ct);
            }
        }
        
        public async UniTask RuntimeRegisterAsync(object obj, CancellationToken ct)
        {
            Register(obj);
            if (obj is IInitializable initializable)
            {
                await initializable.InitAsync(ct);
            }
        }

        public void Register(List<ILifecycleObject> lifecycleObjects)
        {
            foreach (var lifecycleObject in lifecycleObjects)
            {
                Register(lifecycleObject);
            }
        }
        
        public async UniTask RuntimeRegisterAsync(List<ILifecycleObject> lifecycleObjects, CancellationToken ct)
        {
            foreach (var lifecycleObject in lifecycleObjects)
            {
                await RuntimeRegisterAsync(lifecycleObject, ct);
            }
        }

        public T Register<T>(T obj)
        {
            if (obj is IInitializable initializable && !_initializableList.Contains(initializable))
            {
                _initializableList.Add(initializable);
            }
            
            if (obj is ITickable tickable && !_tickableList.Contains(tickable))
            {
                _tickableList.Add(tickable);
            }
            
            if (obj is ILateTickable lateTickable && !_lateTickableList.Contains(lateTickable))
            {
                _lateTickableList.Add(lateTickable);
            }
            
            if (obj is IFixedTickable fixedTickable && !_fixedTickableList.Contains(fixedTickable))
            {
                _fixedTickableList.Add(fixedTickable);
            }
            
            if (obj is IDisposable disposable && !_disposableList.Contains(disposable))
            {
                _disposableList.Add(disposable);
            }
            
            return obj;
        }

        public void Unregister(object obj)
        {
            _pendingUnregister.Add(obj);
        }

        private void Update()
        {
            foreach (var tickable in _tickableList)
            {
                tickable.Tick();
            }
        }

        private void FixedUpdate()
        {
            foreach (var tickable in _fixedTickableList)
            {
                tickable.FixedTick();
            }
        }

        private void LateUpdate()
        {
            foreach (var lateTickable in _lateTickableList)
            {
                lateTickable.LateTick();
            }
            
            FlushUnregister();
        }

        private void FlushUnregister()
        {
            if (_pendingUnregister.Count == 0)
                return;

            foreach (var obj in _pendingUnregister)
            {
                if (obj is IInitializable initializable)
                {
                    _initializableList.Remove(initializable);
                }

                if (obj is ITickable tickable)
                {
                    _tickableList.Remove(tickable);
                }
                
                if (obj is ILateTickable lateTickable)
                {
                    _lateTickableList.Remove(lateTickable);
                }

                if (obj is IFixedTickable fixedTickable)
                {
                    _fixedTickableList.Remove(fixedTickable);
                }

                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                    _disposableList.Remove(disposable);
                }
            }

            _pendingUnregister.Clear();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposableList)
            {
                disposable.Dispose();
            }
            
            _initializableList = null;
            _tickableList = null;
            _lateTickableList = null;
            _fixedTickableList = null;
            _disposableList = null;
        }
    }
}