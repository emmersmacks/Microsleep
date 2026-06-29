using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Core.GameStates
{
    public interface IState
    {
        UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct);

        void Exit();
    }
    
    public interface IGameState : IState
    {
        
    }

	public interface IGlobalState : IState
    {
        
    }
}

