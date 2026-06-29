using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Core.GameStates
{
    public abstract class StateMachineBase<TState> : IStateMachine where TState : IState
    {
        private Dictionary<Type, TState> _states;
        protected TState _currentState;
        
        public StateMachineBase(List<TState> states)
        {
            _states = states.ToDictionary(s => s.GetType(), s => s);
        }
        
        public async UniTask SetStateAsync<T>(CancellationToken ct) where T : IState
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = _states[typeof(T)];

            if (_currentState != null)
            {
                await _currentState.EnterAsync(this, ct);
            }
        }
    }
}