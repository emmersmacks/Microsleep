using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.GlobalStates;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.MainMenu.Menu.StartGameButton
{
    public class MenuButtonsController : WindowControllerBase<MenuButtonsView>, IInitializable
    {
        private CancellationToken _cancellationToken;

        private readonly IEventBus _eventBus;

        public MenuButtonsController(MenuButtonsView view, IEventBus eventBus) : base(view)
        {
            _eventBus = eventBus;
            View.StartButton.onClick.AddListener(StartGame);
            View.CreditsButton.onClick.AddListener(ShowCredits);
            View.ShopButton.onClick.AddListener(ShowShop);
            View.LeaderboardButton.onClick.AddListener(ShowLeaderboard);
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            return UniTask.CompletedTask;
        }

        private void StartGame()
        {
            GlobalStateMachine.Instance.SetStateAsync<GameState>(_cancellationToken).Forget(Debug.LogException);
        }

        private void ShowCredits()
        {
            _eventBus.Publish(new PushWindowRequest<CreditsWindow>());
        }
        
        private void ShowShop()
        {
            _eventBus.Publish(new PushWindowRequest<ShopWindow>());
        }
        
        private void ShowLeaderboard()
        {
            _eventBus.Publish(new PushWindowRequest<LeaderboardWindow>());
        }
    }
}