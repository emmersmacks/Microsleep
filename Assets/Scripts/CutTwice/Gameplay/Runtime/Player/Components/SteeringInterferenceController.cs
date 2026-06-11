using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    internal class SteeringInterferenceController : ITickable
    {
        private readonly SteeringInterferencePresenter _presenter;
        private readonly PlayerSleepController _playerSleepController;

        private float _interference; // -1 .. 1
        private float _targetInterference;
        private float _holdTimer;
        private float _holdDuration;

        public SteeringInterferenceController(SteeringInterferencePresenter presenter, PlayerSleepController playerSleepController)
        {
            _presenter = presenter;
            _playerSleepController = playerSleepController;
        }

        public void Tick()
        {
            float sleep = _playerSleepController.SleepLevel;

            float t = Mathf.InverseLerp(_presenter.sleepStart, _presenter.sleepMax, sleep);
            t = Mathf.Clamp01(t);

            float intensity = _presenter.intensityCurve.Evaluate(t);

            _holdTimer -= Time.deltaTime;
            if (_holdTimer <= 0f)
            {
                _targetInterference = UnityEngine.Random.Range(-1f, 1f);
                float durationMultiplier = Mathf.Lerp(0.5f, 2f, intensity);
                _holdDuration = UnityEngine.Random.Range(_presenter.holdDurationRange.x, _presenter.holdDurationRange.y) * durationMultiplier;
                _holdTimer = _holdDuration;
            }

            float speed = Mathf.Lerp(1f, _presenter.changeSpeed, intensity);
            _interference = Mathf.Lerp(_interference, _targetInterference, Time.deltaTime * speed);
        }

        public float ModifyInput(float input)
        {
            input = Mathf.Clamp(input, -1f, 1f);

            float sleep = _playerSleepController.SleepLevel;
            float t = Mathf.InverseLerp(_presenter.sleepStart, _presenter.sleepMax, sleep);
            t = Mathf.Clamp01(t);

            float intensity = _presenter.intensityCurve.Evaluate(t);
            if (intensity <= 0f) return input;

            if (input != 0f && Mathf.Sign(input) != Mathf.Sign(_interference))
            {
                float target = Mathf.Sign(input) * _presenter.minControl;
                return Mathf.Lerp(input, target, intensity);
            }

            float disturbed = input + _interference * 0.3f * intensity;
            return Mathf.Clamp(disturbed, -1f, 1f);
        }
    }
}



