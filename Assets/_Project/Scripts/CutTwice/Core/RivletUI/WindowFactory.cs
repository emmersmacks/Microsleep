using System;
using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.Factory;
using CutTwice.Core.Lifecycle;
using Cysharp.Threading.Tasks;
using CascadeDI;
using CascadeDI.Container;

namespace CutTwice.Core.RivletUI
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IContainer _container;

        public WindowFactory(IContainer container)
        {
            _container = container;
        }

        public async UniTask<IWindowInstance> CreateAsync(string name, Action<IContainer> compose, CancellationToken ct)
        {
            var lifecycleManager = LifecycleManagerUtils.CreateLifecycleManager(name);
            var childContainer = _container.CreateChild();
            compose(childContainer);
            var scope = childContainer.CreateScope();
            var lifecicleObjects = scope.Resolve<List<ILifecycleObject>>(false);
            await lifecycleManager.RuntimeRegisterAsync(lifecicleObjects, ct);

            var controllers = scope.Resolve(typeof(List<IWindowController>)) as List<IWindowController>;
            controllers ??= new List<IWindowController>();

            return new WindowInstance(controllers, lifecicleObjects, scope, lifecycleManager);
        }
    }
}


