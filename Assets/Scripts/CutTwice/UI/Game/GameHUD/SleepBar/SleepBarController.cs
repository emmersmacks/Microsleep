using CutTwice.Common;
using CutTwice.Controllers;
using CutTwice.Core.RivletUI;

namespace CutTwice.UI.SleepBar
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