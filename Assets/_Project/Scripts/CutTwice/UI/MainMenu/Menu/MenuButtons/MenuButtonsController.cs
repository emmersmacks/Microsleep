using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Menu.States;
using CutTwice.UI.MainMenu.Credits;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.MainMenu.Menu.StartGameButton
{
    public class MenuButtonsController : WindowControllerBase<MenuButtonsView>, IInitializable
    {
        private readonly IEventBus _eventBus;
        private readonly MenuStateMachine _menuStateMachine;
        private CancellationToken _cancellationToken;

        public MenuButtonsController(MenuButtonsView view, IEventBus eventBus, MenuStateMachine menuStateMachine) : base(view)
        {
            _eventBus = eventBus;
            _menuStateMachine = menuStateMachine;
            View.StartButton.onClick.AddListener(StartGame);
            View.SelectMapButton.onClick.AddListener(SelectMap);
            View.CreditsButton.onClick.AddListener(ShowCredits);
            View.ShopButton.onClick.AddListener(ShowShop);
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            return UniTask.CompletedTask;
        }

        private void StartGame()
        {
            _menuStateMachine.NavigateToAsync<SelectLevelState>(_cancellationToken).Forget(Debug.LogException);
        }

        private void SelectMap()
        {
            _menuStateMachine.NavigateToAsync<SelectMapState>(_cancellationToken).Forget(Debug.LogException);
        }

        private void ShowCredits()
        {
            _eventBus.Publish(new PushWindowRequest<CreditsWindow>());
        }

        private void ShowShop()
        {
            _menuStateMachine.NavigateToAsync<ILocationEntryState>(_cancellationToken).Forget(Debug.LogException);
        }
    }
}