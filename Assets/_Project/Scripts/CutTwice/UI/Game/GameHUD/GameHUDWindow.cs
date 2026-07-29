using CascadeDI.Container;
using CutTwice.Core.RivletUI;
using CutTwice.UI.Game.GameHUD.SleepBar;
using CutTwice.UI.Game.GameHUD.TimePanel;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;

namespace CutTwice.UI.Game.GameHUD
{
    public class GameHUDWindow : WindowBase<GameHUDWindowView>
    {
        public GameHUDWindow(GameHUDWindowView windowView, IWindowFactory windowFactory) 
            : base(windowView, windowFactory) { }

        public override void Compose(IContainer builder)
        {
            builder.RegisterSingleton(typeof(SleepBarView), _windowView.SleepBarView);
            builder.RegisterSingletonWithLifetime<SleepBarController>();
            
            builder.RegisterSingleton(typeof(TimePanelView), _windowView.TimePanelView);
            builder.RegisterSingletonWithLifetime<TimePanelController>();
        }
    }
}