using System.Threading;
using Cysharp.Threading.Tasks;

namespace CutTwice.Services
{
    public interface IService
    {
        UniTask InitAsync(CancellationToken cancellationToken = default);
        UniTask DestroyAsync();
    }
}