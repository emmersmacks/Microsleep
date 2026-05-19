using System;
using System.Collections.Generic;
using CutTwice.Common.Infrastructure;
using CutTwice.Controllers;
using CutTwice.Core.RivletUI;
using CutTwice.Infrastructure.Factories;
using CutTwice.ObstacleSequence;
using CutTwice.ObstacleSequence.ModuleLoader;
using CutTwice.ObstacleSequence.Services;
using CutTwice.Scenario;
using CutTwice.Scenario.Act1;
using CutTwice.UI;
using CutTwice.UI.Game.GameOver;
using CutTwice.UI.Game.GameOver.MenuExitButton;
using CutTwice.UI.Game.GameOver.RestartButton;
using CutTwice.UI.SleepBar;
using UnityEngine;
using UnityEngine.Rendering;

namespace CutTwice.Game
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
            // Player
            var playerCarPresenter = _gameSceneReferences.Player.GetComponent<PlayerCarPresenter>();
            var playerCarController = lifecycleManager.Register(new PlayerCarController(playerCarPresenter));

            var playerInputController = lifecycleManager.Register(new PlayerInputController(playerCarController));
            
            var playerSleepPresenter = _gameSceneReferences.Player.GetComponent<PlayerSleepPresenter>();
            var playerSleepController = lifecycleManager.Register(new PlayerSleepController(playerSleepPresenter));
            
            var steeringInterferencePresenter = _gameSceneReferences.Player.GetComponent<SteeringInterferencePresenter>();
            var steeringInterferenceController = lifecycleManager.Register(new SteeringInterferenceController(steeringInterferencePresenter, playerSleepController));
            
            // Scenario
            var initialStage = lifecycleManager.Register(new InitialStage(_gameSceneReferences.Player, _gameSceneReferences.PlayerInitialPosition));
            var openEyeStage = lifecycleManager.Register(new OpenEyeStage(_gameSceneReferences.PostProcessing));
            var scenarioManager = lifecycleManager.Register(new ScenarioManager(0, playerInputController, new ScenarioStage[] { initialStage, openEyeStage }));
            var infiniteRoadController = lifecycleManager.Register(new InfiniteRoadController(_gameSceneReferences.InfiniteRoadPresenter));
            
            // Obstacles
            var trafficFactory = lifecycleManager.Register(new TrafficFactory());
            var deerFactory = lifecycleManager.Register(new DeerFactory());
            
            var addressablesModuleLoader = lifecycleManager.Register(new AddressablesModuleLoader());
            var obstacleSequenceService = lifecycleManager.Register(new ObstacleSequenceService(addressablesModuleLoader));
            var obstacleSequenceBuilder = lifecycleManager.Register(new ObstacleSequenceBuilder(obstacleSequenceService, infiniteRoadController, trafficFactory, deerFactory, lifecycleManager));
            var obstacleRuntimeController = lifecycleManager.Register(new ObstacleRuntimeController());
            
            // Game
            var gameSession = lifecycleManager.Register(new GameSession(obstacleSequenceService, obstacleSequenceBuilder, obstacleRuntimeController));
            
            // UI
            var timePanelController = lifecycleManager.Register(new TimePanelController(_gameSceneReferences.GameHUDView.TimePanelView, gameSession));
            var sleepBarController = lifecycleManager.Register(new SleepBarController(_gameSceneReferences.GameHUDView.SleepBarView, playerSleepController));
            var gameHud = lifecycleManager.Register(new GameHUDWindow(_gameSceneReferences.GameHUDView.gameObject, timePanelController, sleepBarController));

            var exitMenuButtonController = lifecycleManager.Register(new MenuExitButtonController(_gameSceneReferences.GameOverView.ExitMenuButtonView));
            var restartButtonController = lifecycleManager.Register(new RestartButtonController(_gameSceneReferences.GameOverView.RestartButtonView));
            var gameOverWindow = lifecycleManager.Register(new GameOverWindow(_gameSceneReferences.GameOverView.gameObject, exitMenuButtonController, restartButtonController));

            
            var uiManager = lifecycleManager.Register(new UIManager());
            uiManager.Register(gameHud);
            uiManager.Register(gameOverWindow);
        }
    }
}