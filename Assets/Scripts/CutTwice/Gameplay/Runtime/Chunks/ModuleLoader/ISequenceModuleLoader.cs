using System.Threading;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks.ModuleLoader
{
    /// <summary>
    /// Sequence modules loader.
    /// Provides asynchronous methods to retrieve the list of available modules and to load a specific module by name.
    /// </summary>
    public interface ISequenceModuleLoader
    {
        /// <summary>
        /// Asynchronously loads the list of available sequence module previews.
        /// </summary>
        /// <returns>An array of <see cref="SequenceModulePreviewDto"/> representing module previews.</returns>
        public UniTask<SequenceModulePreviewDto[]> LoadModulesListAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Asynchronously loads the chunk.
        /// </summary>
        /// <returns>An array of <see cref="SequenceModulePreviewDto"/> representing module previews.</returns>
        public UniTask<ChunkListDto> LoadChunkAsync(ChunkType chunkType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously loads the full DTO describing a module by its reference.
        /// </summary>
        /// <returns>A <see cref="SequenceModuleDto"/> containing the full module description.</returns>
        public UniTask<SequenceModuleDto> LoadModuleAsync(SequenceModulePreviewDto moduleRef, CancellationToken cancellationToken = default);
    }
}