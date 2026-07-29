using CascadeDI.Container;
using Cinemachine;
using CutTwice.Core.Initialization;
using CutTwice.Core.Lifecycle;

namespace CutTwice.Location.Building
{
    public class SpotCompositionRoot : CompositionRoot
    {
        public CinemachineVirtualCamera Camera;
        
        public override void Compose(IContainer container, RuntimeLifecycleManager lifecycleManager)
        {
            
        }
    }
}