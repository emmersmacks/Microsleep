using System.Threading;
using CutTwice.Core.GameStates;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.GameStates
{
    public class GameLoopState : IGameState
    {
        public UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public void Exit() { }
    }
}

