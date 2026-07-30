using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.RivletUI;
using CutTwice.Menu;
using CutTwice.Services;
using CutTwice.UI.MainMenu.SelectLevel;
using Cysharp.Threading.Tasks;

namespace CutTwice.Menu.States
{
    public class SelectLevelState : IMenuState
    {
        private readonly IEventBus _eventBus;
        private readonly IFadeService _fadeService;
        private readonly MenuCameraSwitcher _cameraSwitcher;
        private readonly StageCamera<SelectLevelState> _stageCamera;

        public SelectLevelState(IEventBus eventBus, IFadeService fadeService, MenuCameraSwitcher cameraSwitcher, StageCamera<SelectLevelState> stageCamera)
        {
            _eventBus = eventBus;
            _fadeService = fadeService;
            _cameraSwitcher = cameraSwitcher;
            _stageCamera = stageCamera;
        }

        public async UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            _cameraSwitcher.SwitchTo(_stageCamera.Camera);
            await _fadeService.FadeOutAsync(ct);
            await UniTask.Delay(200, cancellationToken: ct);
            _cameraSwitcher.CutBlend();

            _eventBus.Publish(new PushWindowRequest<SelectLevelWindow>());
            await _fadeService.FadeInAsync(ct);
        }

        public void Exit()
        {
        }
    }
}
