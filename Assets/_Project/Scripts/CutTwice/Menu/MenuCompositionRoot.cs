using System;
using System.Collections.Generic;
using CutTwice.Core.EventBus;
using CutTwice.Core.Factory;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Location;
using CutTwice.Menu.Initializers;
using CutTwice.Menu.States;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.MapCards;
using CutTwice.UI.MainMenu.MapScreen;
using CutTwice.UI.MainMenu.Menu;
using CascadeDI.Container;
using CutTwice.UI.MainMenu.SelectLevel;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Menu
{
    public class MenuCompositionRoot : CompositionRoot
    {
        public MenuSceneReferences SceneReferences;
        
        public override void Compose(IContainer builder, RuntimeLifecycleManager lifecycleManager)
        {
            builder.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);
            
            var eventBus = new EventBus();
            builder.RegisterSingleton<IEventBus>(eventBus);

            builder.RegisterSingleton<CameraSwitchContext>(SceneReferences.CameraSwitchContext);
            builder.RegisterSingletonWithLifetime<MenuCameraSwitcher>();

            builder.RegisterSingleton<LocationFactory>();

            // UI
            builder.RegisterSingleton(typeof(MenuWindowView), SceneReferences.MenuWindow);
            builder.RegisterSingletonWithLifetime<MenuWindow>(new List<Type>{ typeof(IWindow) });

            builder.RegisterSingleton(typeof(CreditsWindowView), SceneReferences.CreditsWindow);
            builder.RegisterSingletonWithLifetime<CreditsWindow>(new List<Type>{ typeof(IWindow) });
            
            builder.RegisterSingleton(typeof(SelectLevelWindowView), SceneReferences.SelectLevelWindowView);
            builder.RegisterSingletonWithLifetime<SelectLevelWindow>(new List<Type>{ typeof(IWindow) });
            
            builder.RegisterSingleton(typeof(SelectMapWindowView), SceneReferences.SelectMapWindowView);
            builder.RegisterSingletonWithLifetime<SelectMapWindow>(new List<Type>{ typeof(IWindow) });

            builder.RegisterSingleton(typeof(MapCardSlotsView), SceneReferences.MapCardSlots);
            builder.RegisterSingletonWithLifetime<MapCardSlotsController>();

            builder.RegisterSingleton(typeof(MapWindowView), SceneReferences.MapWindowView);
            builder.RegisterSingletonWithLifetime<MapWindow>(new List<Type>{ typeof(IWindow) });

            builder.RegisterSingleton(typeof(LeaderboardWindowView), SceneReferences.LeaderboardWindow);
            builder.RegisterSingletonWithLifetime<LeaderboardWindow>(new List<Type>{ typeof(IWindow) });
            
            builder.RegisterSingleton<IWindowFactory, WindowFactory>();
            builder.RegisterSingletonWithLifetime<UIManager>();

            // Menu States
            builder.RegisterSingletonWithLifetime<MainMenuState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<SelectMapState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<SelectLevelState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<ShopState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<MenuStateMachine>();

            // Initializer
            builder.RegisterSingletonWithLifetime<MenuInitializer>();
        }
    }
}