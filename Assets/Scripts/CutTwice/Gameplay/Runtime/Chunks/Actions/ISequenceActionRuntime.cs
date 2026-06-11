using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Gameplay.Runtime.Chunks.Actions
{
    public interface ISequenceActionRuntime
    {
        public UniTask Init(CancellationToken ct);
        public UniTask Run(CancellationToken ct);
    }
}