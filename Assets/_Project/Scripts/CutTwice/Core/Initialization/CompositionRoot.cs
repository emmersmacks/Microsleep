using CutTwice.Core.Lifecycle;
using CascadeDI.Container;
using UnityEngine;

namespace CutTwice.Core.Initialization
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        public abstract void Compose(IContainer builder, RuntimeLifecycleManager lifecycleManager);
    }
}