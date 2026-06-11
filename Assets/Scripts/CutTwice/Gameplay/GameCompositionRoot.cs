using System.Collections.Generic;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.GameStates;
using CutTwice.Gameplay.Initializers;
using CutTwice.Gameplay.Runtime.Chunks;
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
using CutTwice.UI.Game.GameHUD.SleepBar;
using CutTwice.UI.Game.GameHUD.TimePanel;
using CutTwice.UI.Game.GameOver;
using CutTwice.UI.Game.GameOver.MenuExitButton;
using CutTwice.UI.Game.GameOver.RestartButton;
using UnityEngine;

namespace CutTwice.Gameplay
{
    public class GameCompositionRoot : CompositionRoot
    {
        private readonly GameSceneReferences _gameSceneReferences;

        public GameCompositionRoot(GameSceneReferences gameSceneReferences)
        {
            _gameSceneReferences = gameSceneReferences;
        }

        public override void Compose(RuntimeLifecycleManager lifecycleManager)
        {
            // Event bus (scene-local)
            var eventBus = lifecycleManager.Register(new Core.EventBus.EventBus());

            // Player
            var playerInputController = lifecycleManager.Register(new PlayerInputController(Camera.main));
            
            var playerCarPresenter = _gameSceneReferences.Player.GetComponent<PlayerCarPresenter>();
            var playerCarController = lifecycleManager.Register(new PlayerCarController(playerCarPresenter, playerInputController));

            var playerSleepPresenter = _gameSceneReferences.Player.GetComponent<PlayerSleepPresenter>();
            var playerSleepController = lifecycleManager.Register(new PlayerSleepController(playerSleepPresenter));
            
            var steeringInterferencePresenter = _gameSceneReferences.Player.GetComponent<SteeringInterferencePresenter>();
            var steeringInterferenceController = lifecycleManager.Register(new SteeringInterferenceController(steeringInterferencePresenter, playerSleepController));
            
            // Scenario
            var initialStage = lifecycleManager.Register(new InitialStage(_gameSceneReferences.Player, _gameSceneReferences.PlayerInitialPosition));
            var openEyeStage = lifecycleManager.Register(new OpenEyeStage(_gameSceneReferences.PostProcessing));
            var scenarioManager = lifecycleManager.Register(new ScenarioManager(0, playerInputController, new ScenarioStage[] { initialStage, openEyeStage }));
            var infiniteRoadController = lifecycleManager.Register(new InfiniteRoadController(_gameSceneReferences.InfiniteRoadPresenter));
            
            // Runtime
            var rotateBackviewMirrorController = lifecycleManager.Register(new RotateMirrorController(_gameSceneReferences.RotateBackviewMirrorPresenter, playerInputController));
            lifecycleManager.Register(_gameSceneReferences.BackviewReflectionObjectPresenter);
            var backviewReflectionObjectController = lifecycleManager.Register(new ReflectionObjectController(_gameSceneReferences.BackviewReflectionObjectPresenter, rotateBackviewMirrorController));
            
            var backviewHazardPresenter = lifecycleManager.Register(_gameSceneReferences.BackviewMirrorHazardPresenter);
            var backviewMirrorHazardController = lifecycleManager.Register(new BackviewMirrorHazardController(backviewHazardPresenter, backviewReflectionObjectController, rotateBackviewMirrorController, eventBus));
            
            var rotateSideviewMirrorController = lifecycleManager.Register(new RotateMirrorController(_gameSceneReferences.RotateSideviewMirrorPresenter, playerInputController));
            lifecycleManager.Register(_gameSceneReferences.SideviewReflectionObjectPresenter);
            var sideviewReflectionObjectController = lifecycleManager.Register(new ReflectionObjectController(_gameSceneReferences.SideviewReflectionObjectPresenter, rotateSideviewMirrorController));
            
            var sideviewMirrorHazardController = lifecycleManager.Register(new SideviewMirrorHazardController(_gameSceneReferences.SideviewMirrorHazardPresenter, sideviewReflectionObjectController, rotateSideviewMirrorController, eventBus));

            var leftsideSoundLoopPresenter = lifecycleManager.Register(_gameSceneReferences.LeftSideHazardLoopSoundPresenter);
            lifecycleManager.Register(new MusicLoopController(leftsideSoundLoopPresenter));
            
            // Obstacles
            var trafficFactory = lifecycleManager.Register(new TrafficFactory(eventBus));
            var deerFactory = lifecycleManager.Register(new DeerFactory(eventBus));
            
            var addressablesModuleLoader = lifecycleManager.Register(new AddressablesModuleLoader());
            var obstacleSequenceService = lifecycleManager.Register(new ObstacleSequenceService(addressablesModuleLoader));
            var obstacleSequenceBuilder = lifecycleManager.Register(new ObstacleSequenceBuilder(obstacleSequenceService,
                infiniteRoadController,
                trafficFactory,
                deerFactory,
                lifecycleManager, 
                backviewMirrorHazardController,
                sideviewMirrorHazardController));
            var obstacleRuntimeController = lifecycleManager.Register(new ObstacleRuntimeController());
            
            // Game

            var endGameState = lifecycleManager.Register(new EndGameState(eventBus));
            var gameplayState = lifecycleManager.Register(new GameLoopState());
            var pauseState = lifecycleManager.Register(new PauseGameState());
            var startGameState = lifecycleManager.Register(new StartGameState(eventBus));

            var gameStateMachine = lifecycleManager.Register(new GameStateMachine(new List<IGameState>()
            {
                endGameState,
                gameplayState,
                pauseState,
                startGameState
            }));
            
            var gameSession = lifecycleManager.Register(new GameSession(obstacleSequenceService, obstacleSequenceBuilder, obstacleRuntimeController, eventBus, gameStateMachine));
            
            // UI
            var timePanelController = lifecycleManager.Register(new TimePanelController(_gameSceneReferences.GameHUDView.TimePanelView, gameSession));
            var sleepBarController = lifecycleManager.Register(new SleepBarController(_gameSceneReferences.GameHUDView.SleepBarView, playerSleepController));
            var gameHud = lifecycleManager.Register(new GameHUDWindow(_gameSceneReferences.GameHUDView.gameObject, timePanelController, sleepBarController));

            var exitMenuButtonController = lifecycleManager.Register(new MenuExitButtonController(_gameSceneReferences.GameOverView.ExitMenuButtonView));
            var restartButtonController = lifecycleManager.Register(new RestartButtonController(_gameSceneReferences.GameOverView.RestartButtonView));
            var gameOverWindow = lifecycleManager.Register(new GameOverWindow(_gameSceneReferences.GameOverView.gameObject, exitMenuButtonController, restartButtonController));

            
            var uiManager = lifecycleManager.Register(new UIManager(eventBus));
            uiManager.Register(gameHud);
            uiManager.Register(gameOverWindow);
            
            // Initialization
            var initialization = new GameInitializer(gameStateMachine);
        }
    }
}