using System;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Gameplay.Modes;
using CutTwice.Gameplay.Runtime.Chunks;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Gameplay.Runtime.Chunks.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.GameStates
{
    public class GameLoopState : IGameState
    {
        private readonly IObstacleSequenceService _service;
        private readonly ObstacleSequenceBuilder _builder;
        private readonly ObstacleRuntimeController _runtime;
        private readonly IEventBus _eventBus;
        private readonly SessionTimer _sessionTimer;
        private readonly AdventureFlowService _adventureFlowService;

        private CancellationTokenSource _loopCts;
        private IStateMachine _stateMachine;
        private CancellationToken _appCt;

        public GameLoopState(IObstacleSequenceService service,
            ObstacleSequenceBuilder builder,
            ObstacleRuntimeController runtime,
            IEventBus eventBus,
            SessionTimer sessionTimer,
            AdventureFlowService adventureFlowService)
        {
            _service = service;
            _builder = builder;
            _runtime = runtime;
            _eventBus = eventBus;
            _sessionTimer = sessionTimer;
            _adventureFlowService = adventureFlowService;
        }

        public async UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            _stateMachine = stateMachine;
            _appCt = ct;
            
            _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _eventBus.Subscribe<GameOverEvent>(OnGameOverRequested);
            _eventBus.Subscribe<RunCompletedEvent>(OnRunCompleted);
            _sessionTimer.StartNewRun();

            var sequence = await LoadSequenceAsync(new SequenceModulePreviewDto { Name = "DefaultSequenceModule" }, _loopCts.Token);
            RunLoopAsync(sequence, _loopCts.Token).Forget(Debug.LogException);
        }

        private async UniTask<ObstacleSequenceModuleRuntime> LoadSequenceAsync(SequenceModulePreviewDto modulePreviewDto, CancellationToken ct)
        {
            var dto = await _service.LoadModuleAsync(modulePreviewDto, ct);
            return await _builder.BuildModuleAsync(dto, ct);
        }

        private async UniTask RunLoopAsync(ObstacleSequenceModuleRuntime sequence, CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await _runtime.RunAsync(sequence, true, ct);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void OnGameOverRequested(GameOverEvent evt)
        {
            _sessionTimer.Stop();
            _loopCts.Cancel();
            _adventureFlowService.HandleRunFailed();
            _stateMachine.SetStateAsync<EndGameState>(_appCt).Forget(Debug.LogException);
        }

        private void OnRunCompleted(RunCompletedEvent evt)
        {
            _sessionTimer.Stop();
            _loopCts.Cancel();
            if (!_adventureFlowService.TryHandleRunCompleted(_appCt))
            {
                _stateMachine.SetStateAsync<EndGameState>(_appCt).Forget(Debug.LogException);
            }
        }

        public void Exit()
        {
            _eventBus.Unsubscribe<GameOverEvent>(OnGameOverRequested);
            _eventBus.Unsubscribe<RunCompletedEvent>(OnRunCompleted);
            _loopCts?.Cancel();
            _loopCts?.Dispose();
            _loopCts = null;
        }
    }
}
