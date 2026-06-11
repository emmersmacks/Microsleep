using System.Threading;
using CutTwice.Core.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.GameStates
{
    public class PauseGameState : IGameState
    {
        public UniTask Enter(IStateMachine stateMachine, CancellationToken ct)
        {
            Time.timeScale = 0f;
            return UniTask.CompletedTask;
        }

        public void Exit()
        {
            Time.timeScale = 1f;
        }
    }
}

