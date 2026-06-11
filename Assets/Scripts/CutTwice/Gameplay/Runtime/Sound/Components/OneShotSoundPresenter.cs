using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Sound.Components
{
    [RequireComponent(typeof(AudioSource))]
    public class OneShotSoundPresenter : MonoBehaviour
    {
        [Tooltip("Start playback from this time (seconds)")]
        public float StartTimeInSeconds;
    }
}