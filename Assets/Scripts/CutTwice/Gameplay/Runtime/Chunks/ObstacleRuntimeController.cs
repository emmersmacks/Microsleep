using System.Linq;
using System.Threading;
using CutTwice.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace CutTwice.Gameplay.Runtime.Chunks
{
    public class ObstacleRuntimeController
    {
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
    }
}