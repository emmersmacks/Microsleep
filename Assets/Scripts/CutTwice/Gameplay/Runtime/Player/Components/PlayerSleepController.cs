using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    public class PlayerSleepController : ITickable
    {
        public float SleepLevel;
        
        private readonly PlayerSleepPresenter _presenter;

        public PlayerSleepController(PlayerSleepPresenter presenter)
        {
            _presenter = presenter;
        }

        public void Tick()
        {
            SleepLevel += Time.deltaTime * _presenter.SleepSpeed;
        }
    }
}



