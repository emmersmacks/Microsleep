using System.Collections.Generic;
using System.Threading;
using CascadeDI.Container;
using CutTwice.App.LoadingScreen;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Gameplay.Runtime.Map.Nodes;
using CutTwice.Location;
using CutTwice.Menu.States;
using Cysharp.Threading.Tasks;

namespace CutTwice.Menu.Initializers
{
    public class MenuInitializer : IInitializable
    {
        private const string DefaultLocationPresetId = "BlueSunset";

        private readonly MenuStateMachine _menuStateMachine;
        private readonly LoadingScreenController _loadingScreenController;
        private readonly MapProgressService _mapProgressService;
        private readonly LocationFactory _locationFactory;
        private readonly UIManager _uiManager;
        private readonly IContainer _container;
        private readonly RuntimeLifecycleManager _lifecycleManager;

        public MenuInitializer(
            MenuStateMachine menuStateMachine,
            LoadingScreenController loadingScreenController,
            MapProgressService mapProgressService,
            LocationFactory locationFactory,
            UIManager uiManager,
            IContainer container,
            RuntimeLifecycleManager lifecycleManager)
        {
            _menuStateMachine = menuStateMachine;
            _loadingScreenController = loadingScreenController;
            _mapProgressService = mapProgressService;
            _locationFactory = locationFactory;
            _uiManager = uiManager;
            _container = container;
            _lifecycleManager = lifecycleManager;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            var presetId = _mapProgressService.IsMapActive
                ? ((LocationNode)_mapProgressService.GetLastVisitedLocation()).Preset.Id
                : DefaultLocationPresetId;

            var spot = await _locationFactory.Create(presetId, ct);
            spot.Compose(_container, _lifecycleManager);

            var allWindows = _container.CreateScope().Resolve(typeof(List<IWindow>)) as List<IWindow>;
            var newWindows = _uiManager.RegisterNewWindows(allWindows);

            foreach (var window in newWindows)
            {
                await _lifecycleManager.RuntimeRegisterAsync(window, ct);
            }

            await _menuStateMachine.SetStateAsync<MainMenuState>(ct);
            _loadingScreenController.Hide();
        }
    }
}