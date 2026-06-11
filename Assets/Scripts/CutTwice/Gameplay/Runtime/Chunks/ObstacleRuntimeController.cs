using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CutTwice.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace CutTwice.Gameplay.Runtime.Chunks
{
    public class ObstacleRuntimeController
    {
        private ObstacleSequenceModuleRuntime _sequenceModuleRuntime;
        private SequenceChunkRuntime[] _chunksOrder;
        
        private bool _randomize;
        
        public async Task Init(ObstacleSequenceModuleRuntime sequence, bool randomize, CancellationToken ct)
        {
            foreach (var chunks in sequence.Chunks)
            {
                foreach (var command in chunks.Actions)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                
                    await command.Init(ct);
                }
            }
            
            _sequenceModuleRuntime = sequence;
            _randomize = randomize;
        }

        public async UniTask Run(CancellationToken ct)
        {
            if (_randomize)
            {
                var random = new Random();
                _chunksOrder = _sequenceModuleRuntime.Chunks.Randomize(random).ToArray();
            }
            
            foreach (var chunk in _chunksOrder)
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

            _chunksOrder = null;
        }
    }
}