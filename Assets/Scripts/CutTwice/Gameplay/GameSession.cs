using System;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.GameStates;
using CutTwice.Gameplay.Runtime.Chunks;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Gameplay.Runtime.Chunks.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay
{
    public class GameSession : IInitializable, IDisposable, ITickable
    {
        private readonly IObstacleSequenceService _service;
        private readonly ObstacleSequenceBuilder _builder;
        private readonly ObstacleRuntimeController _runtime;

        public float SessionTime;
        
        private bool _gameStarted;
        private CancellationTokenSource _sessionCancellationTokenSource;
        private CancellationToken _destroyCancellationToken;
        
        private readonly IEventBus _eventBus;
        private readonly IStateMachine _gameStateMachine;

        public GameSession(IObstacleSequenceService service, 
            ObstacleSequenceBuilder builder, 
            ObstacleRuntimeController runtime, 
            IEventBus eventBus,
            GameStateMachine gameStateMachine)
        {
            _service = service;
            _builder = builder;
            _runtime = runtime;
            _eventBus = eventBus;
            _gameStateMachine = gameStateMachine;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            _destroyCancellationToken = ct;
            _sessionCancellationTokenSource = new CancellationTokenSource();
            _eventBus.Subscribe<GameOverEvent>(OnGameOverRequested);
            _gameStarted = true;
            
            var sequence = await LoadSequenceAsync(new SequenceModulePreviewDto { Name = "DefaultSequenceModule" }, _sessionCancellationTokenSource.Token);
            StartSequenceAsync(sequence, true, true, ct).Forget(Debug.LogException);
        }

        private async UniTask<ObstacleSequenceModuleRuntime> LoadSequenceAsync(SequenceModulePreviewDto modulePreviewDto, CancellationToken ct)
        {
            var dto = await _service.LoadModuleAsync(modulePreviewDto, ct);
            return await _builder.BuildModuleAsync(dto, ct);
        }

        private async UniTask StartSequenceAsync(ObstacleSequenceModuleRuntime sequence, bool isLoop, bool randomize, CancellationToken ct)
        {
            try
            {
                do
                {
                    await _runtime.RunAsync(sequence, randomize, ct);
                } while (isLoop && _gameStarted && !ct.IsCancellationRequested);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Sequence cancelled");
            }
        }

        public void Tick()
        {
            if(_gameStarted)
            {
                SessionTime += Time.deltaTime;
            }
        }

        private void OnGameOverRequested(GameOverEvent obj)
        {
            if(!_gameStarted)
                return;
            
            _gameStarted = false;
            
            _sessionCancellationTokenSource.Cancel();
            _sessionCancellationTokenSource.Dispose();
            _sessionCancellationTokenSource = null;

            _gameStateMachine.SetStateAsync<EndGameState>(_destroyCancellationToken).Forget(Debug.LogException);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<GameOverEvent>(OnGameOverRequested);

            if (_sessionCancellationTokenSource != null)
            {
                _sessionCancellationTokenSource.Cancel();
                _sessionCancellationTokenSource.Dispose();
                _sessionCancellationTokenSource = null;
            }
        }
    }
}