using System;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay;

namespace CutTwice.UI.Game.GameHUD.TimePanel
{
    public class TimePanelController : WindowControllerBase<TimePanelView>, ITickable
    {
        private readonly SessionTimer _sessionTimer;

        public TimePanelController(TimePanelView view, SessionTimer sessionTimer) : base(view)
        {
            _sessionTimer = sessionTimer;
        }

        public void Tick()
        {
            TimeSpan t = TimeSpan.FromSeconds(_sessionTimer.SessionTime);
            View.TimeText.text = $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
        }
    }
}