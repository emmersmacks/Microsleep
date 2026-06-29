using System.Threading;
using CutTwice.Core.GameStates;
using Cysharp.Threading.Tasks;

namespace CutTwice.App.GlobalStates
{
    public class BootstrapState : IGlobalState
    {
        public UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    
        public void Exit() { }
    }
}