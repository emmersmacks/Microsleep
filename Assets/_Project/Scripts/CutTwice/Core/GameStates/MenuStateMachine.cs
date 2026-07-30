using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Core.GameStates
{
    public class MenuStateMachine : StateMachineBase<IMenuState>
    {
        private bool _isTransitioning;
        private readonly Stack<Type> _history = new();

        public MenuStateMachine(List<IMenuState> states) : base(states)
        {
        }

        public bool CanGoBack => _history.Count > 0;

        public async UniTask NavigateToAsync<T>(CancellationToken ct) where T : IMenuState
        {
            if (_isTransitioning)
            {
                return;
            }

            _isTransitioning = true;
            try
            {
                var targetState = ResolveState<T>();
                if (_currentState != null && !ReferenceEquals(_currentState, targetState))
                {
                    _history.Push(_currentState.GetType());
                }

                await ApplyStateAsync(targetState, ct);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        public async UniTask GoBackAsync(CancellationToken ct)
        {
            if (_isTransitioning || _history.Count == 0)
            {
                return;
            }

            _isTransitioning = true;
            try
            {
                var previousState = ResolveStateByExactType(_history.Pop());
                await ApplyStateAsync(previousState, ct);
            }
            finally
            {
                _isTransitioning = false;
            }
        }
    }
}
