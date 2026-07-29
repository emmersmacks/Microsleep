using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.RivletUI;
using CutTwice.Menu;
using CutTwice.Services;
using CutTwice.UI.MainMenu.Shop;
using Cysharp.Threading.Tasks;

namespace CutTwice.Menu.States
{
    public class LocationState : IMenuState
    {
        private readonly IEventBus _eventBus;
        private readonly IFadeService _fadeService;
        private readonly MenuCameraSwitcher _cameraSwitcher;

        public LocationState(IEventBus eventBus, IFadeService fadeService, MenuCameraSwitcher cameraSwitcher)
        {
            _eventBus = eventBus;
            _fadeService = fadeService;
            _cameraSwitcher = cameraSwitcher;
        }

        public async UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            _cameraSwitcher.SwitchTo(MenuCameraType.Location);
            await _fadeService.FadeOutAsync(ct);
            await UniTask.Delay(200, cancellationToken: ct);
            _cameraSwitcher.CutBlend();

            _eventBus.Publish(new PushWindowRequest<ShopWindow>());
            await _fadeService.FadeInAsync(ct);
        }

        public void Exit()
        {
        }
    }
    
    public class SelectMapState : IMenuState
    {
        private readonly IEventBus _eventBus;
        private readonly IFadeService _fadeService;
        private readonly MenuCameraSwitcher _cameraSwitcher;

        public SelectMapState(IEventBus eventBus, IFadeService fadeService, MenuCameraSwitcher cameraSwitcher)
        {
            _eventBus = eventBus;
            _fadeService = fadeService;
            _cameraSwitcher = cameraSwitcher;
        }

        public async UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            _cameraSwitcher.SwitchTo(MenuCameraType.SelectMap);
            await _fadeService.FadeOutAsync(ct);
            await UniTask.Delay(200, cancellationToken: ct);
            _cameraSwitcher.CutBlend();

            _eventBus.Publish(new PushWindowRequest<SelectMapWindow>());
            await _fadeService.FadeInAsync(ct);
        }

        public void Exit()
        {
        }
    }
}
