using System;
using System.Collections.Generic;
using CascadeDI.Container;
using CutTwice.Core.EventBus;
using CutTwice.Core.Factory;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.GameStates;
using CutTwice.Gameplay.Modes;
using CutTwice.Gameplay.Runtime.Chunks;
using CutTwice.Gameplay.Runtime.Chunks.Actions;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader;
using CutTwice.Gameplay.Runtime.Chunks.Services;
using CutTwice.Gameplay.Runtime.Hazards.Components;
using CutTwice.Gameplay.Runtime.Interactables.Components;
using CutTwice.Gameplay.Runtime.Player.Components;
using CutTwice.Gameplay.Runtime.Road.Components;
using CutTwice.Gameplay.Runtime.Scenario;
using CutTwice.Gameplay.Runtime.Scenario.Stages;
using CutTwice.Gameplay.Runtime.Sound.Components;
using CutTwice.UI.Game.GameHUD;
using CutTwice.UI.Game.GameOver;

namespace CutTwice.Gameplay
{
    public class GameCompositionRoot : CompositionRoot
    {
        public GameSceneReferences GameSceneReferences;

        public override void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager)
        {
            container.RegisterSingleton<RuntimeLifecycleManager>(lifecycleManager);

            ComposeGameplayModule(container, lifecycleManager);
            ComposeUIModule(container);
            
            // Initialization
            container.RegisterSingletonWithLifetime<GameInitializer>();
        }

        private void ComposeGameplayModule(IContainer builder, RuntimeLifecycleManager lifecycleManager)
        {
            // Services
            var eventBus = new EventBus();
            builder.RegisterSingleton<IEventBus>(eventBus);
            
            // Gameplay
            var playerInputController = lifecycleManager.Register(new PlayerInputController(GameSceneReferences.PlayerCamera));
            builder.RegisterSingleton(typeof(PlayerInputController), playerInputController);
            var playerCarPresenter = GameSceneReferences.Player.GetComponent<PlayerCarPresenter>();
            var playerCarController = lifecycleManager.Register(new PlayerCarController(playerCarPresenter, playerInputController));
            var playerSleepPresenter = GameSceneReferences.Player.GetComponent<PlayerSleepPresenter>();
            var playerSleepController = lifecycleManager.Register(new PlayerSleepController(playerSleepPresenter));
            builder.RegisterSingleton<PlayerSleepController>(playerSleepController);
            var steeringInterferencePresenter = GameSceneReferences.Player.GetComponent<SteeringInterferencePresenter>();
            var steeringInterferenceController = lifecycleManager.Register(new SteeringInterferenceController(steeringInterferencePresenter, playerSleepController));
            
            var infiniteRoadController = lifecycleManager.Register(new InfiniteRoadController(GameSceneReferences.InfiniteRoadPresenter));
            builder.RegisterSingleton<InfiniteRoadController>(infiniteRoadController);
            
            var rotateBackviewMirrorController = lifecycleManager.Register(new RotateMirrorController(GameSceneReferences.RotateBackviewMirrorPresenter, playerInputController));
            var backviewReflectionObjectController = lifecycleManager.Register(new ReflectionObjectController(GameSceneReferences.BackviewReflectionObjectPresenter, rotateBackviewMirrorController));
            var backviewMirrorHazardController = lifecycleManager.Register(new BackviewMirrorHazardController(GameSceneReferences.BackviewMirrorHazardPresenter, backviewReflectionObjectController, rotateBackviewMirrorController, eventBus));
            builder.RegisterSingleton(typeof(BackviewMirrorHazardController), backviewMirrorHazardController);
            
            var rotateSideviewMirrorController = lifecycleManager.Register(new RotateMirrorController(GameSceneReferences.RotateSideviewMirrorPresenter, playerInputController));
            var sideviewReflectionObjectController = lifecycleManager.Register(new ReflectionObjectController(GameSceneReferences.SideviewReflectionObjectPresenter, rotateSideviewMirrorController));
            var sideviewMirrorHazardController = lifecycleManager.Register(new SideviewMirrorHazardController(GameSceneReferences.SideviewMirrorHazardPresenter, sideviewReflectionObjectController, rotateSideviewMirrorController, eventBus));
            var sideviewHazardSoundLoopController = lifecycleManager.Register(new MusicLoopController(GameSceneReferences.LeftSideHazardLoopSoundPresenter));
            builder.RegisterSingleton(typeof(SideviewMirrorHazardController), sideviewMirrorHazardController);


            // Game States
            builder.RegisterSingletonWithLifetime<EndGameState>(new List<Type>{ typeof(IGameState) });
            builder.RegisterSingletonWithLifetime<GameLoopState>(new List<Type>{ typeof(IGameState) });
            builder.RegisterSingletonWithLifetime<PauseGameState>(new List<Type>{ typeof(IGameState) });
            builder.RegisterSingletonWithLifetime<StartGameState>(new List<Type>{ typeof(IGameState) });
            builder.RegisterSingletonWithLifetime<GameStateMachine>();
            
            // Orchestration
            
            // --- Scenario
            var initialStage = lifecycleManager.Register(new InitialStage(GameSceneReferences.Player, GameSceneReferences.PlayerInitialPosition));
            var openEyeStage = lifecycleManager.Register(new OpenEyeStage(GameSceneReferences.PostProcessing));
            var scenarioSystem = new ScenarioSystem(GameSceneReferences.ScenarioManagerSettings, playerInputController, new ScenarioStage[]
            {
                initialStage,
                openEyeStage,
            });
            lifecycleManager.Register(scenarioSystem);
            builder.RegisterSingleton<ScenarioSystem>(scenarioSystem);

            // --- Obstacles
            builder.RegisterTransientWithLifetime<ISequenceActionRuntime, DelayAction>(typeof(DelayAction));
            builder.RegisterTransientWithLifetime<ISequenceActionRuntime, ShowBackViewMirrorObjectAction>(typeof(ShowBackViewMirrorObjectAction));
            builder.RegisterTransientWithLifetime<ISequenceActionRuntime, ShowSideViewMirrorObjectAction>(typeof(ShowSideViewMirrorObjectAction));
            builder.RegisterTransientWithLifetime<ISequenceActionRuntime, SpawnDeerAction>(typeof(SpawnDeerAction));
            builder.RegisterTransientWithLifetime<ISequenceActionRuntime, SpawnTrafficAction>(typeof(SpawnTrafficAction));
            
            builder.RegisterSingleton<ActionFactory>();
            builder.RegisterSingletonWithLifetime<TrafficGameObjectFactory>();
            builder.RegisterSingletonWithLifetime<DeerGameObjectFactory>();
            builder.RegisterSingletonWithLifetime<ISequenceModuleLoader, AddressablesModuleLoader>();
            builder.RegisterSingletonWithLifetime<IObstacleSequenceService, ObstacleSequenceService>();
            builder.RegisterSingletonWithLifetime<ObstacleSequenceBuilder>();
            builder.RegisterSingletonWithLifetime<ObstacleRuntimeController>();
            
            // Session
            builder.RegisterSingletonWithLifetime<SessionTimer>();
            builder.RegisterSingletonWithLifetime<DistanceTracker>();
        }

        private void ComposeUIModule(IContainer builder)
        {
            builder.RegisterSingleton(typeof(GameHUDWindowView), GameSceneReferences.gameHUDWindowView);
            builder.RegisterSingletonWithLifetime<GameHUDWindow>(new List<Type>{ typeof(IWindow) });
            
            builder.RegisterSingleton(typeof(GameOverWindowView), GameSceneReferences.gameOverWindowView);
            builder.RegisterSingletonWithLifetime<GameOverWindow>(new List<Type>{ typeof(IWindow) });

            builder.RegisterSingleton<IWindowFactory, WindowFactory>();
            builder.RegisterSingletonWithLifetime<UIManager>();
        }
    }
}