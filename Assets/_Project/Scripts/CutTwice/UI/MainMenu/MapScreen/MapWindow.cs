using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Common.UIBackButton;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapWindow : WindowBase<MapWindowView>
    {
        public MapWindow(MapWindowView windowView, IWindowFactory windowFactory)
            : base(windowView, windowFactory)
        {
        }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(UIBackButtonView), _windowView.BackButtonView);
            builder.RegisterSingletonWithLifetime<UIBackButtonController>(new List<Type> { typeof(IWindowController) });

            builder.RegisterSingleton(typeof(MapWindowView), _windowView);
            builder.RegisterSingletonWithLifetime<MapScreenController>(new List<Type> { typeof(IWindowController) });
        }
    }
}
