using System;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay;

namespace CutTwice.UI.Game.GameHUD.TimePanel
{
    public class TimePanelController : WindowControllerBase<TimePanelView>, ITickable
    {
        private readonly GameSession _gameSession;

        public TimePanelController(TimePanelView view, GameSession gameSession) : base(view)
        {
            _gameSession = gameSession;
        }

        public void Tick()
        {
            TimeSpan t = TimeSpan.FromSeconds(_gameSession.SessionTime);
            View.TimeText.text = $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
        }
    }
}