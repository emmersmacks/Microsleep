using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.GameStates;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Initializers
{
    public class GameInitializer : IInitializable
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameInitializer(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            await _gameStateMachine.SetStateAsync<StartGameState>(ct);
        }
    }
}