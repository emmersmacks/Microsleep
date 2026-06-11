using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Runtime.Player;
using CutTwice.Gameplay.Runtime.Player.Components;

namespace CutTwice.UI.Game.GameHUD.SleepBar
{
    public class SleepBarController : WindowControllerBase<SleepBarView>, ITickable
    {
        private readonly PlayerSleepController _sleepController;

        public SleepBarController(SleepBarView view, PlayerSleepController sleepController) : base(view)
        {
            _sleepController = sleepController;
        }

        public void Tick()
        {
            View.ProgressBar.fillAmount = _sleepController.SleepLevel / 30f;
        }
    }
}