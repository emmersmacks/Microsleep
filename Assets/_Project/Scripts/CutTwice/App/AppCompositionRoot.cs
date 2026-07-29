using System;
using System.Collections.Generic;
using CutTwice.App.Fade;
using CutTwice.App.GlobalStates;
using CutTwice.App.LoadingScreen;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay;
using CutTwice.Gameplay.GlobalStates;
using CutTwice.Gameplay.Modes;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Menu.GlobalStates;
using CutTwice.Services;
using CascadeDI.Container;

namespace CutTwice.App
{
    public class AppCompositionRoot : CompositionRoot
    {
        public AppSceneReferences SceneReferences;

        public override void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager)
        {
            container.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);
            
            // UI
            container.RegisterSingleton(typeof(LoadingScreenView), SceneReferences.LoadingScreen);
            container.RegisterSingletonWithLifetime<LoadingScreenController>();
            container.RegisterSingleton(typeof(FadeView), SceneReferences.FadeView);

            // Services
            container.RegisterSingletonWithLifetime<PurchaseService>();
            container.RegisterSingletonWithLifetime<AudioSnapshotService>();
            container.RegisterSingletonWithLifetime<FadeService>(new List<Type>{ typeof(IFadeService) });

            // AppStateMachine
            container.RegisterSingleton<IGlobalState, GlobalBootstrapState>();
            container.RegisterSingleton<IGlobalState, GlobalMainMenuState>();
            container.RegisterSingleton<IGlobalState, GlobalGameState>();
            container.RegisterSingleton<GlobalStateMachine>();

            // Player data
            PlayerData.Load();

            // Game mode
            container.RegisterSingleton<GameModeContext>(new GameModeContext());

            // Player maps
            container.RegisterSingleton<PlayerMapsService>(new PlayerMapsService(SceneReferences.HardcodedAdventureMap));

            // Map progress
            container.RegisterSingletonWithLifetime<MapProgressService>();
            container.RegisterSingleton<AdventureFlowService>();

            container.RegisterSingletonWithLifetime<AppInitializer>();
        }
    }
}