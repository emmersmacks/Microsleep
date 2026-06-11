using System.Linq;
using System.Threading;
using CutTwice.Core.Addressables;
using CutTwice.Core.StaticNames;
using CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Chunks.ModuleLoader
{
    public class AddressablesModuleLoader : ISequenceModuleLoader
    {
        async UniTask<SequenceModulePreviewDto[]> ISequenceModuleLoader.LoadModulesListAsync(CancellationToken ct)
        {
            var assets = await AddressablesAsyncLoader.LoadAssetsAsync<TextAsset>(AddressableLabels.SequenceModule, ct);
            return assets.Select(asset => new SequenceModulePreviewDto { Name = asset.name }).ToArray();
        }

        async UniTask<ChunkListDto> ISequenceModuleLoader.LoadChunkAsync(ChunkType chunkType, CancellationToken ct = default)
        {
            var asset = await AddressablesAsyncLoader.LoadAssetAsync<TextAsset>($"{AddressableLabels.ChunkListPrefix}/{chunkType.ToString()}", ct);
            return JsonConvert.DeserializeObject<ChunkListDto>(asset.text);
        }

        async UniTask<SequenceModuleDto> ISequenceModuleLoader.LoadModuleAsync(SequenceModulePreviewDto module, CancellationToken ct)
        {
            var asset = await AddressablesAsyncLoader.LoadAssetAsync<TextAsset>(module.Name, ct);
            return JsonConvert.DeserializeObject<SequenceModuleDto>(asset.text);
        }
    }
}