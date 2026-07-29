using System.Threading;
using CutTwice.Core.Lifecycle;
using CascadeDI.Container;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Core.Initialization
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        public virtual UniTask PreCompose(IContainer parentContainer, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
        public abstract void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager);
    }
}