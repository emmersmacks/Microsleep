using System;
using System.Collections.Generic;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Common.UIBackButton;
using CascadeDI.Container;
using CutTwice.Core.Factory;

namespace CutTwice.UI.MainMenu.Leaderboard
{
    public class LeaderboardWindow : WindowBase<LeaderboardWindowView>
    {
        public LeaderboardWindow(LeaderboardWindowView windowView, IWindowFactory windowFactory) 
            : base(windowView, windowFactory) { }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(UIBackButtonView), _windowView.BackButtonView);
            builder.RegisterSingletonWithLifetime<UIBackButtonController>(new List<Type> { typeof(IWindowController)});
        }
    }
}