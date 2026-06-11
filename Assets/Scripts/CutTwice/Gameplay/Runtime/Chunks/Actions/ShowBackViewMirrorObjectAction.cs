using System.Threading;
using CutTwice.Gameplay.Runtime.Hazards.Components;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    public class ShowBackViewMirrorObjectAction : ISequenceActionRuntime
    {
        private readonly BackviewMirrorHazardController _backviewMirrorHazardController;

        public ShowBackViewMirrorObjectAction(BackviewMirrorHazardController backviewMirrorHazardController)
        {
            _backviewMirrorHazardController = backviewMirrorHazardController;
        }

        public UniTask Init(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask Run(CancellationToken ct)
        {
            _backviewMirrorHazardController.StartHazard();
            return UniTask.CompletedTask;
        }
    }
}