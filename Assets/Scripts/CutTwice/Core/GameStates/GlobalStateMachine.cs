using System;
using System.Collections.Generic;
using CutTwice.App.GlobalStates;

namespace CutTwice.Core.GameStates
{
    /// <summary>
    /// MonoBehaviour singleton that manages game states and routes Update/FixedUpdate calls.
    /// Automatically subscribes to GameOverEvent and transitions to EndGameState.
    /// </summary>
    public class GlobalStateMachine : StateMachineBase<IGlobalState>
    {
        public static GlobalStateMachine Instance { get; private set; }

        public GlobalStateMachine(List<IGlobalState> states) : base(states)
        {
            _currentState = new BootstrapState();
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("There are two statement instances of GameStateMachine");
            }
        }
    }
}

