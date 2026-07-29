using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Modes;
using CutTwice.Menu.States;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Menu.StartGameButton;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.Location.LocationButtons
{
    public class LocationButtonsController : WindowControllerBase<LocationButtonsView>, IInitializable
    {
        private readonly IEventBus _eventBus;
        private readonly MenuStateMachine _menuStateMachine;
        private readonly AdventureFlowService _adventureFlowService;
        private CancellationToken _cancellationToken;

        public LocationButtonsController(LocationButtonsView view, IEventBus eventBus, MenuStateMachine menuStateMachine, AdventureFlowService adventureFlowService) : base(view)
        {
            _eventBus = eventBus;
            _menuStateMachine = menuStateMachine;
            _adventureFlowService = adventureFlowService;
            View.StartButton.onClick.AddListener(StartGame);
            View.BuildingButton.onClick.AddListener(ShowShop);
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            return UniTask.CompletedTask;
        }

        private void StartGame()
        {
            if (!_adventureFlowService.TryStartAdventure(_cancellationToken))
            {
                Debug.LogWarning("Adventure mode: no map or route selected.");
            }
        }

        private void ShowShop()
        {
            _menuStateMachine.TransitionToAsync<ShopState>(_cancellationToken).Forget(Debug.LogException);
        }
    }
}