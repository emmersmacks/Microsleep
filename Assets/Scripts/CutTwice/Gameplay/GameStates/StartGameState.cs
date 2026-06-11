using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Game.GameHUD;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.GameStates
{
    public class StartGameState : IGameState
    {
        private readonly EventBus _eventBus;

        public StartGameState(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async UniTask Enter(IStateMachine stateMachine, CancellationToken ct)
        {
            Time.timeScale = 1f;

            // TODO: Move To Audio System
            //Mixer.TransitionToSnapshots(new []{ Normal, Crash, Menu }, new []{ 0f, 0f, 1f }, 0);

            _eventBus.Publish(new OpenWindowRequest<GameHUDWindow>());

            await stateMachine.SetStateAsync<GameLoopState>(ct);
        }

        public void Exit()
        {
        }
    }
}

