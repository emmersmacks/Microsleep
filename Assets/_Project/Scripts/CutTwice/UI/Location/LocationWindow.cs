using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Location.LocationButtons;

namespace CutTwice.UI.Location
{
    public class LocationWindow : WindowBase<LocationWindowView>
    {
        public LocationWindow(LocationWindowView windowView, IWindowFactory windowFactory) : base(windowView, windowFactory)
        {
        }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(LocationButtonsView), _windowView.LocationButtonsView);
            builder.RegisterSingletonWithLifetime<LocationButtonsController>(new List<Type> { typeof(IWindowController)});
        }
    }
}