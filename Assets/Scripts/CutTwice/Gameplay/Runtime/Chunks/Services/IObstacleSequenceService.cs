using System.Threading;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using CutTwice.Services;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks.Services
{
    public interface IObstacleSequenceService : IService
    {
        UniTask<SequenceModuleDto> LoadModuleAsync(SequenceModulePreviewDto module, CancellationToken ct);
        UniTask<SequenceChunkDto[]> LoadAllChunksAsync(CancellationToken ct);
    }
}