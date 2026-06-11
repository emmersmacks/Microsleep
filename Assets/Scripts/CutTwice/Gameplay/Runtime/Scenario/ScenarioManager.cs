using System;
using System.Threading;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Runtime.Player;
using CutTwice.Gameplay.Runtime.Player.Components;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Scenario
{
    public class ScenarioManager : ITickable, IInitializable
    {
        private Action<ScenarioStage> _stageStarted;
        
        private ScenarioStage _activeStage;
        
        private int _currentStageIndex;
        private int _initialStageIndex;
        
        private readonly PlayerInputController _playerInputController;
        private readonly ScenarioStage[] _stages;

        public ScenarioManager(int initialStageIndex, PlayerInputController playerInputController, ScenarioStage[] stages)
        {
            _initialStageIndex = initialStageIndex;
            _playerInputController = playerInputController;
            _stages = stages;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _currentStageIndex = _initialStageIndex;
            StartNewStage(_stages[_currentStageIndex]);
            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            foreach (var stage in _stages)
            {
                if (stage.IsActive && stage.StageComplete())
                {
                    stage.IsActive = false;
                    
                    var isLastStage = _currentStageIndex == _stages.Length - 1;
                    _activeStage.StageEnd();
                    
                    if (!isLastStage)
                    {
                        _currentStageIndex++;
                        StartNewStage(_stages[_currentStageIndex]);
                    }
                    else
                    {
                        _playerInputController.Enabled = true;
                        _activeStage = null;
                    }
                }
                
                if (stage.IsActive)
                {
                    stage.StageUpdate();
                }
            }
        }

        private void StartNewStage(ScenarioStage stage)
        {
            _activeStage = stage;
            _playerInputController.Enabled = _activeStage.PlayerInputEnabled;
            _activeStage.IsActive = true;
            _activeStage.StageStart();
            _stageStarted?.Invoke(_activeStage);
        }
    }
}