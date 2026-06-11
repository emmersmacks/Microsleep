using System;
using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    public class PlayerCarController : ITickable
    {
        private readonly PlayerCarPresenter _presenter;
        private readonly PlayerInputController _playerInputController;
        private readonly SteeringInterferenceController _steeringInterferenceController;

        private float _currentAngle;
        private float _input;
        private float _modifiedInput;

        public PlayerCarController(PlayerCarPresenter presenter, PlayerInputController playerInputController)
        {
            _presenter = presenter;
            _playerInputController = playerInputController;
        }

        public void Tick()
        {
            _input = _playerInputController.MoveVector.x;
            
            Func<float, float> modifyInput = input => input;
            if (_steeringInterferenceController != null)
                modifyInput = _steeringInterferenceController.ModifyInput;
            
            _modifiedInput = modifyInput(_input);

            Vector3 position = _presenter.transform.position;
            position.x += _modifiedInput * _presenter.moveSpeed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, _presenter.minX, _presenter.maxX);
            _presenter.transform.position = position;

            _input = 0f;

            var input = Mathf.Clamp(_modifiedInput, -1f, 1f);
            float targetAngle = input * _presenter.maxAngle;
            _currentAngle = Mathf.Lerp(_currentAngle, targetAngle, Time.deltaTime * _presenter.smoothSpeed);
            var steeringAngle = _currentAngle;
            
            if (_presenter.steeringWheel != null)
            {
                _presenter.steeringWheel.localRotation = Quaternion.AngleAxis(steeringAngle, _presenter.rotationAxis);
            }
        }
    }
}


