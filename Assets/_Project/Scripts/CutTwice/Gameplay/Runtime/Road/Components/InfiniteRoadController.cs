using System;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Obstacles.Components;
using UnityEngine;
using UnityEngine.Events;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    public class InfiniteRoadController : ITickable
    {
        public float MovementSpeed => _presenter.moveSpeed;
        private readonly InfiniteRoadPresenter _presenter;
        private readonly RuntimeLifecycleManager _lifecycleManager;

        private bool _startStop;
        private bool stopped;

        public UnityEvent<Transform> OnSegmentSpawned = new();

        public InfiniteRoadController(InfiniteRoadPresenter presenter, RuntimeLifecycleManager lifecycleManager)
        {
            _presenter = presenter;
            _lifecycleManager = lifecycleManager;
            _startStop = false;
        }

        public void SpawnCrossroadFork(Action onPassed)
        {
            if (_presenter.CrossroadForkPrefab == null)
            {
                Debug.LogWarning("InfiniteRoadController: CrossroadForkPrefab is not assigned.");
                return;
            }

            void Handler(Transform segment)
            {
                OnSegmentSpawned.RemoveListener(Handler);

                var instance = UnityEngine.Object.Instantiate(_presenter.CrossroadForkPrefab, segment);
                var stripPresenter = instance.GetComponent<RaycastStripPresenter>();

                RaycastStripController controller = null;
                controller = new RaycastStripController(stripPresenter, () =>
                {
                    _lifecycleManager.Unregister(controller);
                    UnityEngine.Object.Destroy(instance);
                    onPassed?.Invoke();
                });

                _lifecycleManager.Register(controller);
            }

            OnSegmentSpawned.AddListener(Handler);
        }

        public void Tick()
        {
            if (stopped)
                return;

            foreach (Transform segment in _presenter.roadSegments)
            {
                if (segment.position.z <= _presenter.recycleZ)
                {
                    float maxZ = FindMaxZ();
                    OnSegmentSpawned?.Invoke(segment);
                    segment.position = new Vector3(segment.position.x, segment.position.y, maxZ + _presenter.segmentLength);
                }

                segment.Translate(Vector3.back * _presenter.moveSpeed * Time.deltaTime);
            }
        }

        private float FindMaxZ()
        {
            float maxZ = float.MinValue;
            foreach (Transform segment in _presenter.roadSegments)
            {
                if (segment.position.z > maxZ)
                    maxZ = segment.position.z;
            }
            return maxZ;
        }
    }
}



