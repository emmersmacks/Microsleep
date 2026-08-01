using System;
using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Obstacles.Components
{
    public class RaycastStripController : IFixedTickable
    {
        private readonly RaycastStripPresenter _presenter;
        private readonly Action _onHit;

        private Vector3 _previousPosition;
        private bool _hasPrevious;

        public RaycastStripController(RaycastStripPresenter presenter, Action onHit)
        {
            _presenter = presenter;
            _hasPrevious = false;
            _onHit = onHit;
        }

        public void FixedTick()
        {
            Vector3 currentPosition = _presenter.transform.position;

            if (!_hasPrevious)
            {
                _previousPosition = currentPosition;
                _hasPrevious = true;
                return;
            }

            Vector3 delta = _previousPosition - currentPosition;
            float distance = delta.magnitude;
            if (distance <= _presenter.MinMovementThreshold)
            {
                _previousPosition = currentPosition;
                return;
            }

            Vector3 rayDirection = (_previousPosition - currentPosition) / distance;
            Vector3 worldCenter = _presenter.transform.TransformPoint(_presenter.Origin);
            Vector3 worldDir = _presenter.transform.TransformDirection(_presenter.Direction).normalized;
            if (worldDir.sqrMagnitude <= Mathf.Epsilon) worldDir = _presenter.transform.right;
            Vector3 stripStart = worldCenter - worldDir * (_presenter.Width * 0.5f);

            bool hitInvoked = false;
            for (int i = 0; i < _presenter.Count; i++)
            {
                float t = (_presenter.Count == 1) ? 0.5f : (float)i / (_presenter.Count - 1);
                Vector3 rayOrigin = stripStart + worldDir * (t * _presenter.Width);

                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, distance, _presenter.LayerMask, _presenter.TriggerInteraction))
                {
                    if (!hitInvoked)
                    {
                        _onHit?.Invoke();
                        hitInvoked = true;
                    }

                    if (_presenter.DebugDraw)
                        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * distance, _presenter.DebugColorHit, Time.fixedDeltaTime);
                }
                else
                {
                    if (_presenter.DebugDraw)
                        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * distance, _presenter.DebugColorNoHit, Time.fixedDeltaTime);
                }
            }

            _previousPosition = currentPosition;
        }
    }
}



