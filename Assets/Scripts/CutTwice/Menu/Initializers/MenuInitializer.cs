using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.UI.MainMenu.Menu;
using Cysharp.Threading.Tasks;

namespace CutTwice.Menu.Initializers
{
    public class MenuInitializer : IInitializable
    {
        private readonly IEventBus _eventBus;

        public MenuInitializer(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public UniTask InitAsync(CancellationToken ct)
        {
            _eventBus.Publish(new OpenWindowRequest<MenuWindow>());
            return UniTask.CompletedTask;
        }
    }
}