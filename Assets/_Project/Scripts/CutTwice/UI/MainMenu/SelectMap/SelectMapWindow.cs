using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.MainMenu.SelectLevel.SmoothBackButton;

namespace CutTwice.UI.MainMenu.Shop
{
    public class SelectMapWindow : WindowBase<SelectMapWindowView>
    {
        public SelectMapWindow(SelectMapWindowView windowView, IWindowFactory windowFactory) 
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