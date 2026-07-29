using System.Threading;
using Cysharp.Threading.Tasks;
using CascadeDI.Container;

namespace CutTwice.Core.Factory
{
    public interface IWindowFactory
    {
        UniTask<IWindowInstance> CreateAsync(string name, System.Action<IContainer> compose, CancellationToken ct);
    }
}


