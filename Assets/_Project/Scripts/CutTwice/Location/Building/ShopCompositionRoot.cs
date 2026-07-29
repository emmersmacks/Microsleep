using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Location.Building
{
    public class ShopCompositionRoot : SpotCompositionRoot
    {
        public ShopWindowView ShopWindow;
        
        public override void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager)
        {
            // UI
            container.RegisterSingleton(typeof(ShopWindowView), ShopWindow);
            container.RegisterSingletonWithLifetime<ShopWindow>(new List<Type>{ typeof(IWindow) });
        }
    }
}