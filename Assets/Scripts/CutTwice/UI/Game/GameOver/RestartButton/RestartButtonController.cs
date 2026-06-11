using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.GlobalStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.Game.GameOver.RestartButton
{
    public class RestartButtonController : WindowControllerBase<RestartButtonView>, IInitializable
    {
        private CancellationToken _cancellationToken;
        
        public RestartButtonController(RestartButtonView view) : base(view)
        {
            View.RestartButton.onClick.AddListener(RestartGame);
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            return UniTask.CompletedTask;
        }

        private void RestartGame()
        {
            GlobalStateMachine.Instance.SetStateAsync<GameState>(_cancellationToken).Forget(Debug.LogException);
        }
    }
}