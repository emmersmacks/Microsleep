using System;
using CutTwice.Core.EventBus;
using CutTwice.Gameplay.Runtime.Interactables.Components;
using DG.Tweening;

namespace CutTwice.Gameplay.Runtime.Hazards.Components
{
    public class BackviewMirrorHazardController : IDisposable
    {
        private readonly BackviewMirrorHazardPresenter _presenter;
        private readonly ReflectionObjectController _reflectionObjectController;
        private readonly RotateMirrorController _rotateMirrorController;
        private readonly EventBus _eventBus;

        private Tweener _tweener;
        const float duration = 5f;

        public BackviewMirrorHazardController(BackviewMirrorHazardPresenter presenter, ReflectionObjectController reflectionObjectController, RotateMirrorController rotateMirrorController, EventBus eventBus)
        {
            _presenter = presenter;
            _reflectionObjectController = reflectionObjectController;
            _rotateMirrorController = rotateMirrorController;
            _eventBus = eventBus;
        }

        public void StartHazard()
        {
            if (_tweener != null)
            {
                throw new Exception("Cannot start hazard while it is active.");
            }
            
            _presenter.AudioSource.Play();
            
            var t = 0f;
            
            _tweener = DOTween.To(() => t, x =>
            {
                t = x;
            }, 1f, duration).OnComplete(OnHazardComplete);
            
            _reflectionObjectController.ShowReflectionObject(duration);
        }

        private void OnHazardComplete()
        {
            if (!_rotateMirrorController.IsRotating)
            {
                _eventBus.Publish(new GameOverEvent());
            }
            
            _presenter.AudioSource.Stop();
            _tweener = null;
            _reflectionObjectController.HideReflectionObject();
        }

        public void Dispose()
        {
            _tweener.Kill();
            _tweener = null;
        }
    }
}

