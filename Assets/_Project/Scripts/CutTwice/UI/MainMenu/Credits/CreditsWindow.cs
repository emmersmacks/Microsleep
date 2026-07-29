using System;
using System.Collections.Generic;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Common.UIBackButton;
using CascadeDI.Container;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;

namespace CutTwice.UI.MainMenu.Credits
{
    public class CreditsWindow : WindowBase<CreditsWindowView>
    {
        public CreditsWindow(CreditsWindowView windowView, IWindowFactory windowFactory) 
            : base(windowView, windowFactory) { }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(UIBackButtonView), _windowView.BackButtonView);
            builder.RegisterSingletonWithLifetime<UIBackButtonController>(new List<Type> { typeof(IWindowController)});
        }
    }
}