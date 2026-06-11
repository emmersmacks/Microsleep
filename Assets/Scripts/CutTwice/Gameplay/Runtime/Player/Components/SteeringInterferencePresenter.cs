using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    [RequireComponent(typeof(PlayerSleepPresenter))]
    public class SteeringInterferencePresenter : MonoBehaviour
    {
        [Header("Settings")]
        public float sleepStart = 10f;
    
        public float sleepMax = 20f;
    
        public float minControl = 0.2f;
    
        public float interferenceSpeed = 2f;
    
        public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Random Drift")]
        public Vector2 holdDurationRange = new Vector2(0.5f, 2.5f);
        public float changeSpeed = 3f;
    }
}