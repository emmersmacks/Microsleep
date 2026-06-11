using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Menu.Initializers;
using CutTwice.UI.Common.UIBackButton;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.Menu;
using CutTwice.UI.MainMenu.Menu.StartGameButton;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Menu
{
    public class MenuCompositionRoot : CompositionRoot
    {
        private readonly MenuSceneReferences _sceneReferences;

        public MenuCompositionRoot(MenuSceneReferences sceneReferences)
        {
            _sceneReferences = sceneReferences;
        }
        
        public override void Compose(RuntimeLifecycleManager lifecycleManager)
        {
            // Event bus (scene-local)
            var eventBus = lifecycleManager.Register(new CutTwice.Core.EventBus.EventBus());

            // UI
            var menuButtonsController = new MenuButtonsController(_sceneReferences.MenuWindow.MenuButtonsView, eventBus);
            var menuWindow = lifecycleManager.Register(new MenuWindow(_sceneReferences.MenuWindow.gameObject, menuButtonsController));

            var creditsBackButtonController = new UIBackButtonController(_sceneReferences.CreditsWindow.BackButtonView, eventBus);
            var creditsWindow = lifecycleManager.Register(new CreditsWindow(_sceneReferences.CreditsWindow.gameObject, creditsBackButtonController));
            
            var shopBackButtonController = new UIBackButtonController(_sceneReferences.ShopWindow.BackButtonView, eventBus);
            var shopWindow = lifecycleManager.Register(new ShopWindow(_sceneReferences.ShopWindow.gameObject, shopBackButtonController));
            
            var leaderBoardBackButtonController = new UIBackButtonController(_sceneReferences.LeaderboardWindow.BackButtonView, eventBus);
            var leaderBoardWindow = lifecycleManager.Register(new LeaderboardWindow(_sceneReferences.LeaderboardWindow.gameObject, leaderBoardBackButtonController));
            
            var uiManager = lifecycleManager.Register(new UIManager(eventBus));
            uiManager.Register(menuWindow);
            uiManager.Register(creditsWindow);
            uiManager.Register(shopWindow);
            uiManager.Register(leaderBoardWindow);
            
            // Initializer
            var initializers = lifecycleManager.Register(new MenuInitializer(eventBus));
        }
    }
}