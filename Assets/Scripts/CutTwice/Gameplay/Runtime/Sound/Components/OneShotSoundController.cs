using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Sound.Components
{
    public class OneShotSoundController
    {
        private readonly OneShotSoundPresenter _presenter;
        private readonly AudioSource _audioSource;

        public OneShotSoundController(OneShotSoundPresenter presenter)
        {
            _presenter = presenter;
            _audioSource = _presenter.GetComponent<AudioSource>();
        }

        public void Play()
        {
            _audioSource.time = _presenter.StartTimeInSeconds;
            _audioSource.Play();
        }

        public void PlayClip(AudioClip clip)
        {
            _audioSource.time = _presenter.StartTimeInSeconds;
            _audioSource.PlayOneShot(clip);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }
    }
}



