using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.RivletUI;
using CutTwice.Menu;
using CutTwice.Services;
using CutTwice.UI.Location;
using CutTwice.UI.MainMenu.Menu;
using Cysharp.Threading.Tasks;

namespace CutTwice.Location.States
{
    public class MainLocationState : IMenuState
    {
        private readonly IEventBus _eventBus;
        private readonly IFadeService _fadeService;
        private readonly MenuCameraSwitcher _cameraSwitcher;

        public MainLocationState(IEventBus eventBus, IFadeService fadeService, MenuCameraSwitcher cameraSwitcher)
        {
            _eventBus = eventBus;
            _fadeService = fadeService;
            _cameraSwitcher = cameraSwitcher;
        }

        public async UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            _cameraSwitcher.SwitchTo(MenuCameraType.Main);
            
            await _fadeService.FadeOutAsync(ct);
            await UniTask.Delay(200, cancellationToken: ct);
            _cameraSwitcher.CutBlend();

            _eventBus.Publish(new PushWindowRequest<LocationWindow>());
            await _fadeService.FadeInAsync(ct);
        }

        public void Exit()
        {
            
        }
    }
}