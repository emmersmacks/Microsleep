using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.MainMenu.SelectLevel.SelectLevelButtons;
using CutTwice.UI.MainMenu.SelectLevel.SmoothBackButton;

namespace CutTwice.UI.MainMenu.SelectLevel
{
    public class SelectLevelWindow : WindowBase<SelectLevelWindowView>
    {
        public SelectLevelWindow(SelectLevelWindowView windowView, IWindowFactory windowFactory) : base(windowView, windowFactory)
        {
        }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(SmoothBackButtonView), _windowView.BackButtonView);
            builder.RegisterSingletonWithLifetime<SmoothBackButtonController>(new List<Type> { typeof(IWindowController)});
            
            builder.RegisterSingleton(typeof(SelectLevelButtonsView), _windowView.SelectLevelButtonsView);
            builder.RegisterSingletonWithLifetime<SelectLevelButtonsController>(new List<Type> { typeof(IWindowController)});
        }
    }
}