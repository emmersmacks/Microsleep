using CutTwice.UI.SleepBar;
using UnityEngine;

namespace CutTwice.UI
{
    public class GameHUDWindow : WindowBase
    {
        public GameHUDWindow(GameObject windowObject, TimePanelController timePanelController,
            SleepBarController sleepBarController) : base(windowObject)
        {
            Register(timePanelController);
            Register(sleepBarController);
        }
    }
}