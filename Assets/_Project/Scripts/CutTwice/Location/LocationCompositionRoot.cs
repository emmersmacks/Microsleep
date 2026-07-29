using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.EventBus;
using CutTwice.Core.Factory;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Location.States;
using CutTwice.Menu;
using CutTwice.Menu.States;
using CutTwice.UI.Location;

namespace CutTwice.Location
{
    public class LocationCompositionRoot : CompositionRoot
    {
        public LocationWindowView LocationWindow;
        public CameraSwitchContext CameraSwitchContext;

        public override void Compose(IContainer builder, RuntimeLifecycleManager lifecycleManager)
        {
            builder.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);
            builder.RegisterSingleton<LocationFactory>();

            var eventBus = new EventBus();
            builder.RegisterSingleton<IEventBus>(eventBus);
            
            builder.RegisterSingleton<CameraSwitchContext>(CameraSwitchContext);
            builder.RegisterSingletonWithLifetime<MenuCameraSwitcher>();
            
            // Location States
            builder.RegisterSingletonWithLifetime<MainLocationState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<ShopState>(new List<Type>{ typeof(IMenuState) });
            builder.RegisterSingletonWithLifetime<MenuStateMachine>();
            
            // UI
            builder.RegisterSingleton(typeof(LocationWindowView), LocationWindow);
            builder.RegisterSingletonWithLifetime<LocationWindow>(new List<Type>{ typeof(IWindow) });
            
            builder.RegisterSingleton<IWindowFactory, WindowFactory>();
            builder.RegisterSingletonWithLifetime<UIManager>();
            
            builder.RegisterSingletonWithLifetime<LocationInitializer>();
        }
    }
}