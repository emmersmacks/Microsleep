using System.Collections.Generic;
using CutTwice.App.GlobalStates;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay;
using CutTwice.Gameplay.GlobalStates;
using CutTwice.Menu.GlobalStates;
using CutTwice.Services;

namespace CutTwice.App
{
    public class AppCompositionRoot : CompositionRoot
    {
        public override void Compose(RuntimeLifecycleManager lifecycleManager)
        {
            // Services
            var purchaseService = lifecycleManager.Register(new PurchaseService());

            // Event bus
            var eventBus = lifecycleManager.Register(new Core.EventBus.EventBus());

            // UI
            var uiManager = lifecycleManager.Register(new UIManager(eventBus));

            // AppStateMachine
            var bootstrapState = lifecycleManager.Register(new BootstrapState());
            var mainMenuState = lifecycleManager.Register(new MainMenuState());
            var gameState = lifecycleManager.Register(new GameState());

            var stateMachine = lifecycleManager.Register(new GlobalStateMachine(new List<IGlobalState>()
            {
                bootstrapState,
                mainMenuState,
                gameState,
            }));
            
            // Player data
            PlayerData.Load();
        }
    }
}