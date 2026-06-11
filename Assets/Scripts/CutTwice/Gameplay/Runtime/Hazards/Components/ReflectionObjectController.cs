using System;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Interactables.Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CutTwice.Gameplay.Runtime.Hazards.Components
{
    public class ReflectionObjectController : IInitializable, IDisposable
    {
        private readonly ReflectionObjectPresenter _presenter;
        private readonly RotateMirrorController _rotateMirrorController;
        
        private Tweener _reflectionTweener = null;

        public ReflectionObjectController(ReflectionObjectPresenter presenter, RotateMirrorController rotateMirrorController)
        {
            _presenter = presenter;
            _rotateMirrorController = rotateMirrorController;
            //_rotateMirrorController.OnRotateBack += OnRotateBack;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            HideReflectionObject();
            return UniTask.CompletedTask;
        }

        //private void OnRotateBack()
        //{
        //    if (_reflectionTweener != null)
        //    {
        //        _reflectionTweener.ChangeValues(0.5f, 1f, 0.5f);
        //    }
        //}

        public void ShowReflectionObject(float duration)
        {
            if (_reflectionTweener != null)
            {
                throw new Exception("Cannot show reflection object while it is active.");
            }
            
            _reflectionTweener = _presenter.Material.DOFade(1f, duration);
        }

        public void HideReflectionObject()
        {
            _reflectionTweener.Kill();
            _reflectionTweener = null;
            
            var color = _presenter.Material.color;
            color.a = 0f;
            _presenter.Material.color = color;
        }

        public void Dispose()
        {
            _reflectionTweener.Kill();
            _reflectionTweener = null;
            
            //_rotateMirrorController.OnRotateBack -= OnRotateBack;
        }
    }
}