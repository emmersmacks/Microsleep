using System;
using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.Factory;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Location;
using CutTwice.Menu.Initializers;
using CutTwice.Menu.States;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.MapCards;
using CutTwice.UI.MainMenu.MapScreen;
using CutTwice.UI.MainMenu.Menu;
using CascadeDI.Container;
using CutTwice.Core.Addressables;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Gameplay.Runtime.Map.Nodes;
using CutTwice.Location.Building;
using CutTwice.UI.MainMenu.SelectLevel;
using CutTwice.UI.MainMenu.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CutTwice.Menu
{
    public class MenuCompositionRoot : CompositionRoot
    {
        private const string DefaultLocationPresetId = "BlueSunset";
        
        public MenuSceneReferences SceneReferences;
        
        private SpotCompositionRoot _spotCompositionRoot;

        public override async UniTask PreCompose(IContainer parentContainer, CancellationToken ct)
        {
            var parentScope = parentContainer.CreateScope();
            var _mapProgressService = parentScope.Resolve(typeof(MapProgressService)) as MapProgressService;
            var presetId = _mapProgressService.IsMapActive
                ? ((LocationNode)_mapProgressService.GetLastVisitedLocation()).Preset.Id
                : DefaultLocationPresetId;
            
            var locationPrefab = await AddressablesAsyncLoader.LoadAssetAsync<GameObject>(
                $"Location/{presetId}", ct);

            var instance = Instantiate(
                locationPrefab,
                locationPrefab.transform.position,
                locationPrefab.transform.rotation);

            _spotCompositionRoot = instance.GetComponentInChildren<SpotCompositionRoot>(true);
            SceneReferences.CameraSwitchContext.Cameras.Add(new CameraSwitchContext.CameraData(){ Camera = _spotCompositionRoot.Camera, CameraType = MenuCameraType.Location});
        }

        public override void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager)
        {
            container.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);
            
            var eventBus = new EventBus();
            container.RegisterSingleton<IEventBus>(eventBus);

            container.RegisterSingleton<CameraSwitchContext>(SceneReferences.CameraSwitchContext);
            container.RegisterSingletonWithLifetime<MenuCameraSwitcher>();

            _spotCompositionRoot.Compose(container, lifecycleManager);

            // UI
            container.RegisterSingleton(typeof(MenuWindowView), SceneReferences.MenuWindow);
            container.RegisterSingletonWithLifetime<MenuWindow>(new List<Type>{ typeof(IWindow) });

            container.RegisterSingleton(typeof(CreditsWindowView), SceneReferences.CreditsWindow);
            container.RegisterSingletonWithLifetime<CreditsWindow>(new List<Type>{ typeof(IWindow) });
            
            container.RegisterSingleton(typeof(SelectLevelWindowView), SceneReferences.SelectLevelWindowView);
            container.RegisterSingletonWithLifetime<SelectLevelWindow>(new List<Type>{ typeof(IWindow) });
            
            container.RegisterSingleton(typeof(SelectMapWindowView), SceneReferences.SelectMapWindowView);
            container.RegisterSingletonWithLifetime<SelectMapWindow>(new List<Type>{ typeof(IWindow) });

            container.RegisterSingleton(typeof(MapCardSlotsView), SceneReferences.MapCardSlots);
            container.RegisterSingletonWithLifetime<MapCardSlotsController>();

            container.RegisterSingleton(typeof(MapWindowView), SceneReferences.MapWindowView);
            container.RegisterSingletonWithLifetime<MapWindow>(new List<Type>{ typeof(IWindow) });

            container.RegisterSingleton(typeof(LeaderboardWindowView), SceneReferences.LeaderboardWindow);
            container.RegisterSingletonWithLifetime<LeaderboardWindow>(new List<Type>{ typeof(IWindow) });
            
            container.RegisterSingleton<IWindowFactory, WindowFactory>();
            container.RegisterSingletonWithLifetime<UIManager>();

            // Menu States
            container.RegisterSingletonWithLifetime<MainMenuState>(new List<Type>{ typeof(IMenuState) });
            container.RegisterSingletonWithLifetime<SelectMapState>(new List<Type>{ typeof(IMenuState) });
            container.RegisterSingletonWithLifetime<SelectLevelState>(new List<Type>{ typeof(IMenuState) });
            container.RegisterSingletonWithLifetime<LocationState>(new List<Type>{ typeof(IMenuState) });
            container.RegisterSingletonWithLifetime<MenuStateMachine>();

            // Initializer
            container.RegisterSingletonWithLifetime<MenuInitializer>();
        }
    }
}