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

        public override void Compose(IContainer builder, RuntimeLifecycleManager lifecycleManager)
        {
            builder.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);
            
            // UI
            builder.RegisterSingleton(typeof(LoadingScreenView), SceneReferences.LoadingScreen);
            builder.RegisterSingletonWithLifetime<LoadingScreenController>();
            builder.RegisterSingleton(typeof(FadeView), SceneReferences.FadeView);

            // Services
            builder.RegisterSingletonWithLifetime<PurchaseService>();
            builder.RegisterSingletonWithLifetime<AudioSnapshotService>();
            builder.RegisterSingletonWithLifetime<FadeService>(new List<Type>{ typeof(IFadeService) });

            // AppStateMachine
            builder.RegisterSingleton<IGlobalState, GlobalBootstrapState>();
            builder.RegisterSingleton<IGlobalState, GlobalMainMenuState>();
            builder.RegisterSingleton<IGlobalState, GlobalGameState>();
            builder.RegisterSingleton<IGlobalState, GlobalLocationState>();
            builder.RegisterSingleton<GlobalStateMachine>();

            // Player data
            PlayerData.Load();

            // Game mode
            builder.RegisterSingleton<GameModeContext>(new GameModeContext());

            // Player maps
            builder.RegisterSingleton<PlayerMapsService>(new PlayerMapsService(SceneReferences.HardcodedAdventureMap));

            // Map progress
            builder.RegisterSingletonWithLifetime<MapProgressService>();
            builder.RegisterSingleton<AdventureFlowService>();

            builder.RegisterSingletonWithLifetime<AppInitializer>();
        }
    }
}