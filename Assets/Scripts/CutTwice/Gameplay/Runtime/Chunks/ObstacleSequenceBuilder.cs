using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CutTwice.Core.Lifecycle;
using CutTwice.Gameplay.Factories;
using CutTwice.Gameplay.Runtime.Chunks.Actions;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Gameplay.Runtime.Chunks.Services;
using CutTwice.Gameplay.Runtime.Hazards.Components;
using CutTwice.Gameplay.Runtime.Road.Components;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks
{
    public class ObstacleSequenceBuilder
    {
        private readonly IObstacleSequenceService _service;
        private readonly InfiniteRoadController _infiniteRoadController;
        private readonly TrafficFactory _trafficFactory;
        private readonly DeerFactory _deerFactory;
        private readonly RuntimeLifecycleManager _lifecycleManager;
        private readonly BackviewMirrorHazardController _backviewMirrorHazardController;
        private readonly SideviewMirrorHazardController _sideviewMirrorHazardController;

        private Dictionary<string, SequenceActionDto[]> _allChunks;
        
        public ObstacleSequenceBuilder(IObstacleSequenceService service, InfiniteRoadController infiniteRoadController,
            TrafficFactory trafficFactory, DeerFactory deerFactory, RuntimeLifecycleManager lifecycleManager, BackviewMirrorHazardController backviewMirrorHazardController, SideviewMirrorHazardController sideviewMirrorHazardController)
        {
            _service = service;
            _infiniteRoadController = infiniteRoadController;
            _trafficFactory = trafficFactory;
            _deerFactory = deerFactory;
            _lifecycleManager = lifecycleManager;
            _backviewMirrorHazardController = backviewMirrorHazardController;
            _sideviewMirrorHazardController = sideviewMirrorHazardController;
        }

        public async UniTask Init(CancellationToken ct)
        {
            var allChunks = await _service.LoadAllChunksAsync(ct);
            _allChunks = allChunks.ToDictionary(k => k.Id, v => v.Actions);
        }
        
        public ObstacleSequenceModuleRuntime BuildModule(SequenceModuleDto module)
        {
            var moduleRuntime = new ObstacleSequenceModuleRuntime();
            var chunks = new List<SequenceChunkRuntime>();
            
            foreach(var chunkRef in module.Sequences)
            {
                var chunkRuntime = new SequenceChunkRuntime
                {
                    Name = chunkRef.Id
                };

                var actionDtoArr = _allChunks[chunkRef.Id];
                foreach (var actionDto in actionDtoArr)
                {
                    switch (actionDto.Type)
                    {
                        case ActionType.Delay:
                            chunkRuntime.Actions.Add(new DelayAction(actionDto.Parameters.ToObject<DelayAction.Parameters>()));
                            break;
                        case ActionType.SpawnTraffic:
                            var spawnParameters = actionDto.Parameters.ToObject<SpawnTrafficAction.Parameters>();
                            chunkRuntime.Actions.Add(new SpawnTrafficAction(spawnParameters, _trafficFactory, _lifecycleManager));
                            break;
                        case ActionType.SpawnDeer:
                            var spawnOnTileParameters = actionDto.Parameters.ToObject<SpawnDeerAction.Parameters>();
                            chunkRuntime.Actions.Add(new SpawnDeerAction(spawnOnTileParameters, _infiniteRoadController, _deerFactory, _lifecycleManager));
                            break;
                        case ActionType.BackviewObject:
                            chunkRuntime.Actions.Add(new ShowBackViewMirrorObjectAction(_backviewMirrorHazardController));
                            break;
                        case ActionType.SideviewObject:
                            chunkRuntime.Actions.Add(new ShowSideViewMirrorObjectAction(_sideviewMirrorHazardController));
                            break;
                    }
                }
                chunks.Add(chunkRuntime);
            }
            
            moduleRuntime.Chunks = chunks.ToArray();
            return moduleRuntime;
        }
    }
}