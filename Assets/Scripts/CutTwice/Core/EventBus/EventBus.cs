using System;
using System.Collections.Generic;

namespace CutTwice.Core.EventBus
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T evt);
        void ClearAll();
        bool HasSubscribers<T>();
    }

    /// <summary>
    /// Instance-based, thread-safe EventBus. Register one instance in your CompositionRoot
    /// and inject `IEventBus` into consumers.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly object _lock = new();

        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var type = typeof(T);
                if (!_subscribers.TryGetValue(type, out var list))
                {
                    list = new List<Delegate>();
                    _subscribers[type] = list;
                }

                if (!list.Contains(handler))
                    list.Add(handler);
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var type = typeof(T);
                if (!_subscribers.TryGetValue(type, out var list)) return;

                list.Remove(handler);
                if (list.Count == 0)
                    _subscribers.Remove(type);
            }
        }

        public void Publish<T>(T evt)
        {
            List<Delegate> handlersCopy = null;
            lock (_lock)
            {
                if (_subscribers.TryGetValue(typeof(T), out var list))
                {
                    handlersCopy = new List<Delegate>(list);
                }
            }

            if (handlersCopy == null || handlersCopy.Count == 0)
                return;

            List<Exception> exceptions = null;
            foreach (var d in handlersCopy)
            {
                try
                {
                    ((Action<T>)d)(evt);
                }
                catch (Exception ex)
                {
                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);
                }
            }

            if (exceptions != null)
                throw new AggregateException("One or more EventBus handlers threw exceptions.", exceptions);
        }

        public void ClearAll()
        {
            lock (_lock)
            {
                _subscribers.Clear();
            }
        }

        public bool HasSubscribers<T>()
        {
            lock (_lock)
            {
                return _subscribers.TryGetValue(typeof(T), out var list) && list.Count > 0;
            }
        }
    }
}

