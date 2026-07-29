using System;
using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.RivletUI
{
    /// <summary>
    /// Central manager for registered windows. Does NOT instantiate windows — it only manages already-created controllers.
    /// Windows are registered by a strongly-typed identifier TWindow.
    /// UIManager subscribes to Open/Close requests for registered windows on the EventBus.
    /// Do not register in global scope
    /// </summary>
    public class UIManager : IInitializable, IDisposable
    {
        private readonly Dictionary<Type, RegisteredWindow> _registry = new();
        private readonly List<Type> _stack = new();
        
        private Action<PopWindowRequest> _globalPopHandler;
        private readonly IEventBus _eventBus;

        public UIManager(List<IWindow> windows, IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            RegisterNewWindows(windows);
        }

        /// <summary>
        /// Registers windows whose concrete runtime type isn't known to this manager yet
        /// (skips ones already registered instead of throwing) and returns only the ones
        /// that were actually added. Used for the initial IWindow batch and for windows
        /// composed later at runtime (e.g. a spot-specific window like ShopWindow).
        /// </summary>
        public List<IWindow> RegisterNewWindows(IEnumerable<IWindow> windows)
        {
            var added = new List<IWindow>();
            foreach (var window in windows)
            {
                var type = window.GetType();
                if (_registry.ContainsKey(type))
                    continue;

                typeof(UIManager).GetMethod(nameof(Register))!
                    .MakeGenericMethod(type)
                    .Invoke(this, new object[] { window });

                added.Add(window);
            }
            return added;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            // subscribe to global pop request (untyped)
            _globalPopHandler = _ => Pop();
            _eventBus.Subscribe(_globalPopHandler);
            return UniTask.CompletedTask;
        }

        public void Register<TWindow>(TWindow window) where TWindow : IWindow
        {
            var type = window.GetType();
            if (_registry.ContainsKey(type))
                throw new InvalidOperationException($"Window type {type} already registered");

            // create typed handlers so we can subscribe/unsubscribe later
            Action<OpenWindowRequest<TWindow>> openHandler = req => window.Show(req?.Payload);
            Action<CloseWindowRequest<TWindow>> closeHandler = _ => window.Hide();
            Action<PushWindowRequest<TWindow>> pushHandler = req => Push<TWindow>(req?.Payload);
            Action<PopToWindowRequest<TWindow>> popToHandler = _ => PopTo<TWindow>();

            _eventBus.Subscribe(openHandler);
            _eventBus.Subscribe(closeHandler);
            _eventBus.Subscribe(pushHandler);
            _eventBus.Subscribe(popToHandler);

            _registry[type] = new RegisteredWindow
            {
                Window = window,
                Unsubscribe = () =>
                {
                    _eventBus.Unsubscribe(openHandler);
                    _eventBus.Unsubscribe(closeHandler);
                    _eventBus.Unsubscribe(pushHandler);
                    _eventBus.Unsubscribe(popToHandler);
                }
            };
        }

        public void Unregister<TWindow>() where TWindow : IWindow
        {
            var key = typeof(TWindow);
            if (!_registry.TryGetValue(key, out var reg))
                return;

            reg.Unsubscribe?.Invoke();

            // dispose controller
            _registry.Remove(key);
        }

        // Push - hide current top and show requested window, push it to stack
        public void Push<TWindow>(object payload = null) where TWindow : IWindow
        {
            var key = typeof(TWindow);
            if (!_registry.TryGetValue(key, out var reg))
                throw new KeyNotFoundException($"Window {key} is not registered");

            // if already top - simply show with payload
            if (_stack.Count > 0 && _stack[_stack.Count - 1] == key)
            {
                reg.Window.Show(payload);
                return;
            }

            // hide previous top
            if (_stack.Count > 0)
            {
                var prev = _stack[_stack.Count - 1];
                if (_registry.TryGetValue(prev, out var prevReg))
                    prevReg.Window.Hide();
            }

            // remove previous occurrence if exists (bring to top)
            var existingIndex = _stack.IndexOf(key);
            if (existingIndex >= 0)
                _stack.RemoveAt(existingIndex);

            _stack.Add(key);
            reg.Window.Show(payload);
        }

        // Pop - hide top and show previous if exists
        public void Pop()
        {
            if (_stack.Count == 0)
                return;

            var top = _stack[_stack.Count - 1];
            if (_registry.TryGetValue(top, out var topReg))
            {
                topReg.Window.Hide();
            }

            _stack.RemoveAt(_stack.Count - 1);

            if (_stack.Count > 0)
            {
                var newTop = _stack[_stack.Count - 1];
                if (_registry.TryGetValue(newTop, out var newTopReg))
                {
                    newTopReg.Window.Show();
                }
            }
        }

        // PopTo - pop until target window becomes top (inclusive behavior: target remains visible)
        public void PopTo<TWindow>() where TWindow : IWindow
        {
            var key = typeof(TWindow);
            var idx = _stack.LastIndexOf(key);
            if (idx < 0)
                return; // target not in stack

            // pop until we reach target at top
            while (_stack.Count - 1 > idx)
                Pop();
        }

        public void Show<TWindow>(object payload = null) where TWindow : IWindow
        {
            var key = typeof(TWindow);
            if (!_registry.TryGetValue(key, out var reg))
                throw new KeyNotFoundException($"Window {key} is not registered");

            reg.Window.Show(payload);
        }

        public void Hide<TWindow>() where TWindow : IWindow
        {
            var key = typeof(TWindow);
            if (!_registry.TryGetValue(key, out var reg))
                return;

            reg.Window.Hide();
        }

        public void Dispose()
        {
            foreach (var reg in _registry.Values)
            {
                reg.Unsubscribe?.Invoke();
            }
            
            _registry.Clear();
            if (_globalPopHandler != null)
                _eventBus.Unsubscribe(_globalPopHandler);
        }

        private class RegisteredWindow
        {
            public IWindow Window;
            public Action Unsubscribe;
        }
    }
}




