using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
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

        private bool _goalReached;

        public float DistanceMeters { get; private set; }

        public DistanceTracker(InfiniteRoadController roadController, SessionTimer sessionTimer, GameModeContext gameModeContext, IEventBus eventBus)
        {
            _roadController = roadController;
            _sessionTimer = sessionTimer;
            _gameModeContext = gameModeContext;
            _eventBus = eventBus;
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
                _eventBus.Publish(new RunCompletedEvent());
            }
        }
    }
}
