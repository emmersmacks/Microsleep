using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay
{
    public class SessionTimer : ITickable
    {
        public float SessionTime { get; private set; }
        public bool IsRunning { get; private set; }

        public void StartNewRun()
        {
            SessionTime = 0f;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Tick()
        {
            if (IsRunning)
            {
                SessionTime += Time.deltaTime;
            }
        }
    }
}
