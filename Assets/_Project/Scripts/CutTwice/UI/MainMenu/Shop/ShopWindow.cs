using System;
using System.Collections.Generic;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.UI.MainMenu.SelectLevel.SmoothBackButton;

namespace CutTwice.UI.MainMenu.Shop
{
    public class ShopWindow : WindowBase<ShopWindowView>
    {
        public ShopWindow(ShopWindowView windowView, IWindowFactory windowFactory) 
            : base(windowView, windowFactory)
        {
        }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(SmoothBackButtonView), _windowView.BackButtonView);
            builder.RegisterSingletonWithLifetime<SmoothBackButtonController>(new List<Type> { typeof(IWindowController)});
        }
    }
}