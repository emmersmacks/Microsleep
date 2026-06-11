using System;
using CutTwice.Core.EventBus;
using CutTwice.Gameplay.Runtime.Interactables.Components;
using DG.Tweening;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Hazards.Components
{
    public class SideviewMirrorHazardController : IDisposable
    {
        private readonly SideviewMirrorHazardPresenter _presenter;
        private readonly ReflectionObjectController _reflectionObjectController;
        private readonly RotateMirrorController _rotateMirrorController;
        private readonly EventBus _eventBus;

        private int intensityID;
        
        private Tweener _tweener;
        const float duration = 5f;

        public SideviewMirrorHazardController(SideviewMirrorHazardPresenter presenter, ReflectionObjectController reflectionObjectController, RotateMirrorController rotateMirrorController, EventBus eventBus)
        {
            _presenter = presenter;
            _reflectionObjectController = reflectionObjectController;
            _rotateMirrorController = rotateMirrorController;
            _eventBus = eventBus;
            intensityID = Shader.PropertyToID("_Intensity");
            HideHazardous();
        }

        public void StartHazard()
        {
            if (_tweener != null)
            {
                throw new Exception("Cannot start hazard while it is active.");
            }
            
            var t = 0f;
            
            _presenter.AudioSource.Play();
            
            _tweener = DOTween.To(() => t, x =>
            {
                t = x;

                _presenter.LightMaterial.SetFloat(intensityID, Mathf.Lerp(0f, 6f, t));
                _presenter.Light.intensity = Mathf.Lerp(0f, 3.2f, t);

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
            HideHazardous();
            _reflectionObjectController.HideReflectionObject();
        }

        private void HideHazardous()
        {
            _presenter.LightMaterial.SetFloat(intensityID, 0f);
            _presenter.Light.intensity = 0f;
        }

        public void Dispose()
        {
            _tweener.Kill();
            _tweener = null;
        }
    }
}