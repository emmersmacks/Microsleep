using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Core.GameStates
{
    public abstract class StateMachineBase<TState> : IStateMachine where TState : IState
    {
        private readonly Dictionary<Type, TState> _states;
        private readonly Dictionary<Type, TState> _interfaceStates;
        protected TState _currentState;

        public StateMachineBase(List<TState> states)
        {
            _states = states.ToDictionary(s => s.GetType(), s => s);
            _interfaceStates = BuildInterfaceIndex(states);
        }

        private static Dictionary<Type, TState> BuildInterfaceIndex(List<TState> states)
        {
            var index = new Dictionary<Type, TState>();
            foreach (var state in states)
            {
                var markers = state.GetType().GetInterfaces()
                    .Where(i => i != typeof(TState) && typeof(TState).IsAssignableFrom(i));
                foreach (var marker in markers)
                {
                    if (index.TryGetValue(marker, out var existing))
                    {
                        throw new InvalidOperationException(
                            $"Multiple {typeof(TState).Name} implementations registered for marker '{marker.Name}': " +
                            $"'{existing.GetType().Name}' and '{state.GetType().Name}'. Only one may be active per session.");
                    }
                    index.Add(marker, state);
                }
            }
            return index;
        }

        protected TState ResolveState<T>() where T : IState
        {
            if (_states.TryGetValue(typeof(T), out var byType))
            {
                return byType;
            }

            if (_interfaceStates.TryGetValue(typeof(T), out var byMarker))
            {
                return byMarker;
            }

            throw new KeyNotFoundException($"No {typeof(TState).Name} registered for '{typeof(T).Name}'.");
        }

        protected TState ResolveStateByExactType(Type concreteType) => _states[concreteType];

        protected async UniTask ApplyStateAsync(TState nextState, CancellationToken ct)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = nextState;

            if (_currentState != null)
            {
                await _currentState.EnterAsync(this, ct);
            }
        }

        public async UniTask SetStateAsync<T>(CancellationToken ct) where T : IState
        {
            await ApplyStateAsync(ResolveState<T>(), ct);
        }
    }
}
