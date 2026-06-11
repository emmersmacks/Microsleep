using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Sound.Components
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicLoopPresenter : MonoBehaviour
    {
        public AudioSource AudioSource;

        public float LoopStartTime = 10f;
        public float LoopEndTime = 30f;
    }
}