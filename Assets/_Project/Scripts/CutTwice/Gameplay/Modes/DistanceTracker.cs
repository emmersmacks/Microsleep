using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Gameplay.Runtime.Map.Nodes;
using CutTwice.Gameplay.Runtime.Player.Components;
using CutTwice.Gameplay.Runtime.Road.Components;
using UnityEngine;

namespace CutTwice.Gameplay.Modes
{
    public class DistanceTracker : ITickable
    {
        private readonly InfiniteRoadController _roadController;
        private readonly SessionTimer _sessionTimer;
        private readonly GameModeContext _gameModeContext;
        private readonly IEventBus _eventBus;
        private readonly MapProgressService _mapProgressService;
        private readonly PlayerCarPresenter _playerCarPresenter;

        private bool _goalReached;

        public float DistanceMeters { get; private set; }

        public DistanceTracker(InfiniteRoadController roadController, SessionTimer sessionTimer, GameModeContext gameModeContext, IEventBus eventBus,
            MapProgressService mapProgressService, PlayerCarPresenter playerCarPresenter)
        {
            _roadController = roadController;
            _sessionTimer = sessionTimer;
            _gameModeContext = gameModeContext;
            _eventBus = eventBus;
            _mapProgressService = mapProgressService;
            _playerCarPresenter = playerCarPresenter;
        }

        public void Tick()
        {
            if (!_sessionTimer.IsRunning || _goalReached)
                return;

            DistanceMeters += _roadController.MovementSpeed * Time.deltaTime;

            if (_gameModeContext.CurrentMode is DistanceModeConfig distanceMode &&
                DistanceMeters >= distanceMode.TargetMeters)
            {
                _goalReached = true;

                var nextTarget = _mapProgressService.IsMapActive ? _mapProgressService.GetNextTargetLocation() : null;
                if (nextTarget is CrossroadNode)
                {
                    _roadController.SpawnCrossroadFork(OnCrossroadForkResolved);
                }
                else
                {
                    _eventBus.Publish(new RunCompletedEvent());
                }
            }
        }

        private void OnCrossroadForkResolved()
        {
            bool isLeft = _playerCarPresenter.IsOnLeftSide;

            _mapProgressService.MarkLocationCompleted();
            var exits = _mapProgressService.GetRuntimeState().AvailableExits;
            var chosen = isLeft ? exits[0] : exits[1];

            _mapProgressService.ClearRoute();
            _mapProgressService.AddTargetLocation(chosen.Target);

            DistanceMeters = 0f;
            _goalReached = false;
        }
    }
}
