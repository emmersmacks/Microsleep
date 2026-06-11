using System.Threading;
using CutTwice.Gameplay.Runtime.Hazards.Components;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    public class ShowSideViewMirrorObjectAction : ISequenceActionRuntime
    {
        private readonly SideviewMirrorHazardController _sideviewMirrorHazardController;

        public ShowSideViewMirrorObjectAction(SideviewMirrorHazardController sideviewMirrorHazardController)
        {
            _sideviewMirrorHazardController = sideviewMirrorHazardController;
        }

        public UniTask Init(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask Run(CancellationToken ct)
        {
            _sideviewMirrorHazardController.StartHazard();
            return UniTask.CompletedTask;
        }
    }
}