using System;
using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.Factory;
using Cysharp.Threading.Tasks;
using CascadeDI.Container;

namespace CutTwice.Core.RivletUI
{
    public abstract class WindowBase<TView> : IWindow, IInitializable, IDisposable where TView : WindowViewBase
    {
        protected readonly TView _windowView;
        protected List<IWindowController> Controllers = new();
        private readonly IWindowFactory _windowFactory;
        private IWindowInstance _windowInstance;
        
        public WindowBase(TView windowView, IWindowFactory windowFactory)
        {
            _windowView = windowView;
            _windowFactory = windowFactory;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            _windowView.gameObject.SetActive(false);

            _windowInstance = await _windowFactory.CreateAsync(GetType().Name, Compose, ct);
            Controllers = _windowInstance.Controllers;
        }

        public abstract void Compose(IContainer builder);

        public void Show(object payload = null)
        {
            _windowView.gameObject.SetActive(true);
            foreach (var controller in Controllers)
            {
                controller.Show(payload);
            }
        }

        public void Hide()
        {
            foreach (var controller in Controllers)
            {
                controller.Hide();
            }
            _windowView.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _windowInstance?.Release();
        }
    }
}