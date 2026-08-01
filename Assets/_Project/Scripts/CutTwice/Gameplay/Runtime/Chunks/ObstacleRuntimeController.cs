using System.Linq;
using System.Threading;
using CutTwice.Core;
using CutTwice.Gameplay.Modes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace CutTwice.Gameplay.Runtime.Chunks
{
    public class ObstacleRuntimeController
    {
        private const float SpawnGateRemainingMeters = 100f;

        private readonly DistanceTracker _distanceTracker;
        private readonly GameModeContext _gameModeContext;

        public ObstacleRuntimeController(DistanceTracker distanceTracker, GameModeContext gameModeContext)
        {
            _distanceTracker = distanceTracker;
            _gameModeContext = gameModeContext;
        }

        public async UniTask RunAsync(ObstacleSequenceModuleRuntime sequence, bool randomize, CancellationToken ct)
        {
            SequenceChunkRuntime[] chunksOrder = null;
            if (randomize)
            {
                var random = new Random();
                chunksOrder = sequence.Chunks.Randomize(random).ToArray();
            }

            foreach (var chunk in chunksOrder)
            {
                await UniTask.WaitUntil(ShouldSpawnChunks, cancellationToken: ct);

                Debug.Log("[TEST] Start Chunk: " + chunk.Name);

                foreach (var command in chunk.Actions)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                
                    await command.Run(ct);
                }
            }
        }

        private bool ShouldSpawnChunks()
        {
            if (_gameModeContext.CurrentMode is DistanceModeConfig distanceMode)
            {
                return distanceMode.TargetMeters - _distanceTracker.DistanceMeters >= SpawnGateRemainingMeters;
            }

            return true;
        }
    }
}