using System.Threading;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Sound.Components
{
    public class MusicLoopController : IInitializable, ITickable
    {
        private readonly MusicLoopPresenter _presenter;
        private bool introFinished;
        
        public MusicLoopController(MusicLoopPresenter presenter)
        {
            _presenter = presenter;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            if (_presenter.AudioSource == null)
                _presenter.AudioSource = _presenter.GetComponent<AudioSource>();

            _presenter.AudioSource.loop = false;
            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            if (!_presenter.AudioSource.isPlaying)
                return;

            if (_presenter.AudioSource.time >= _presenter.LoopEndTime)
            {
                _presenter.AudioSource.time = _presenter.LoopStartTime;
                introFinished = true;
            }
        }
    }
}