using System;
using System.Linq;
using System.Threading;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Player.Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Interactables.Components
{
    public class RotateMirrorController : ITickable, IInitializable
    {
        private readonly RotateMirrorPresenter _presenter;
        private readonly PlayerInputController _playerInputController;
        
        public Action OnRotateBack; 
        public bool IsRotating;
        
        private Quaternion _startRotation;
        private Tween _rotationTween;
        private bool _isHolding;
        private bool _isPointerOver;
        private float _duration = 0.1f;
        
        public UniTask InitAsync(CancellationToken ct)
        {
            _startRotation = _presenter.MirrorTransform.rotation;
            return UniTask.CompletedTask;
        }

        public RotateMirrorController(RotateMirrorPresenter presenter, PlayerInputController playerInputController)
        {
            _presenter = presenter;
            _playerInputController = playerInputController;
        }

        public void Tick()
        {
            _isPointerOver = _playerInputController.Hits.Select(h => h.transform).Contains(_presenter.transform);
            if (_isPointerOver)
            {
                _isHolding = true;
                RotateToPressed();
            }
            
            if (_isHolding)
            {
                if (!_isPointerOver)
                {
                    Release();
                }
            }
        }

        private void RotateToPressed()
        {
            _rotationTween?.Kill();
            IsRotating = true;

            _rotationTween = _presenter.MirrorTransform.DORotateQuaternion(
                _startRotation * Quaternion.Euler(_presenter.Pressedrotation),
                _duration);
        }

        private void Release()
        {
            _isHolding = false;
            IsRotating = false;

            _rotationTween?.Kill();

            _rotationTween = _presenter.MirrorTransform.DORotateQuaternion(
                _startRotation,
                _duration);
        }
    }
}