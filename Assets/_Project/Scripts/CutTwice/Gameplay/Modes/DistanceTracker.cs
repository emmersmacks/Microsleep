using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Road.Components;
using UnityEngine;

namespace CutTwice.Gameplay.Modes
{
    public class DistanceTracker : ITickable
    {
        private readonly SessionTimer _sessionTimer;
        private readonly InfiniteRoadPresenter _infiniteRoadPresenter;

        private bool _paused;

        public float DistanceMeters { get; private set; }

        public DistanceTracker(SessionTimer sessionTimer, InfiniteRoadPresenter infiniteRoadPresenter)
        {
            _sessionTimer = sessionTimer;
            _infiniteRoadPresenter = infiniteRoadPresenter;
        }

        public void Tick()
        {
            if (!_sessionTimer.IsRunning || _paused)
                return;

            DistanceMeters += _infiniteRoadPresenter.moveSpeed * Time.deltaTime;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Resume()
        {
            _paused = false;
        }

        public void ResetDistance()
        {
            DistanceMeters = 0f;
        }
    }
}
