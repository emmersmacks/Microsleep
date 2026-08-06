using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Gameplay.Runtime.Map.Nodes;
using CutTwice.Gameplay.Runtime.Obstacles.Components;
using CutTwice.Gameplay.Runtime.Player.Components;
using CutTwice.Gameplay.Runtime.Road.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Modes
{
    /// <summary>
    /// Owns everything about reaching the current run's distance goal and, if the map has a
    /// crossroad next, spawning the fork segment and resolving which branch the player took.
    /// Reads DistanceTracker.DistanceMeters but never writes to it except via ResetDistance().
    /// </summary>
    public class CrossroadRouteController : ITickable
    {
        private readonly InfiniteRoadController _roadController;
        private readonly DistanceTracker _distanceTracker;
        private readonly SessionTimer _sessionTimer;
        private readonly GameModeContext _gameModeContext;
        private readonly IEventBus _eventBus;
        private readonly MapProgressService _mapProgressService;
        private readonly PlayerCarPresenter _playerCarPresenter;
        private readonly RuntimeLifecycleManager _lifecycleManager;

        private bool _goalReached;

        public CrossroadRouteController(InfiniteRoadController roadController, DistanceTracker distanceTracker, SessionTimer sessionTimer,
            GameModeContext gameModeContext, IEventBus eventBus, MapProgressService mapProgressService,
            PlayerCarPresenter playerCarPresenter, RuntimeLifecycleManager lifecycleManager)
        {
            _roadController = roadController;
            _distanceTracker = distanceTracker;
            _sessionTimer = sessionTimer;
            _gameModeContext = gameModeContext;
            _eventBus = eventBus;
            _mapProgressService = mapProgressService;
            _playerCarPresenter = playerCarPresenter;
            _lifecycleManager = lifecycleManager;
        }

        public void Tick()
        {
            if (!_sessionTimer.IsRunning || _goalReached)
                return;

            if (_gameModeContext.CurrentMode is DistanceModeConfig distanceMode &&
                _distanceTracker.DistanceMeters >= distanceMode.TargetMeters)
            {
                _goalReached = true;

                var nextTarget = _mapProgressService.IsMapActive ? _mapProgressService.GetNextTargetLocation() : null;
                if (nextTarget is CrossroadNode)
                {
                    _distanceTracker.Pause();
                    SpawnCrossroadAsync().Forget(Debug.LogException);
                }
                else
                {
                    _eventBus.Publish(new RunCompletedEvent());
                }
            }
        }

        private async UniTask SpawnCrossroadAsync()
        {
            //TODO: Load prefab from node data
            await _roadController.SetOneShotSegmentAsync("Road/TwoWayCrossroad");
            _roadController.OnSegmentSpawned.AddListener(OnCrossroadForkSpawned);
        }

        private void OnCrossroadForkSpawned(RoadSegmentPresenter forkSegment)
        {
            _roadController.OnSegmentSpawned.RemoveListener(OnCrossroadForkSpawned);
            var stripPresenter = forkSegment.GetComponent<RaycastStripPresenter>();

            RaycastStripController controller = null;
            controller = new RaycastStripController(stripPresenter, () =>
            {
                _lifecycleManager.Unregister(controller);
                OnCrossroadForkResolved(forkSegment);
            });

            _lifecycleManager.Register(controller);
        }

        private void OnCrossroadForkResolved(RoadSegmentPresenter forkSegment)
        {
            bool isLeft = _playerCarPresenter.IsOnLeftSide;
            _roadController.ResolveFork(forkSegment, isLeft ? 0 : 1);

            _mapProgressService.MarkLocationCompleted();
            var exits = _mapProgressService.GetRuntimeState().AvailableExits;
            var chosen = isLeft ? exits[0] : exits[1];

            _mapProgressService.ClearRoute();
            _mapProgressService.AddTargetLocation(chosen.Target);

            _distanceTracker.ResetDistance();
            _distanceTracker.Resume();
            _goalReached = false;
        }
    }
}
