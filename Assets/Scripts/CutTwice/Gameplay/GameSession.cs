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
            IStateMachine gameStateMachine)
        {
            _service = service;
            _builder = builder;
            _runtime = runtime;
            _eventBus = eventBus;
            _gameStateMachine = gameStateMachine;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _destroyCancellationToken = ct;
            _sessionCancellationTokenSource = new CancellationTokenSource();
            _eventBus.Subscribe<GameOverEvent>(OnGameOverRequested);
            StartSequenceAsync(new SequenceModulePreviewDto { Name = "DefaultSequenceModule" }, true, true, _sessionCancellationTokenSource.Token).Forget(Debug.LogException);
            return UniTask.CompletedTask;
        }

        private async UniTask StartSequenceAsync(SequenceModulePreviewDto modulePreviewDto, bool isLoop, bool randomize,
            CancellationToken ct)
        {
            _gameStarted = true;
            
            var dto = await _service.LoadModuleAsync(modulePreviewDto, ct);
            await _builder.Init(ct);
            
            var sequence = _builder.BuildModule(dto);
            await _runtime.Init(sequence, randomize, ct);

            do
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                
                await _runtime.Run(ct);
            } while (isLoop);
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