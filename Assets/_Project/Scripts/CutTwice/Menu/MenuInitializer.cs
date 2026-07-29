using System.Threading;
using CutTwice.App.LoadingScreen;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Menu.States;
using Cysharp.Threading.Tasks;

namespace CutTwice.Menu.Initializers
{
    public class MenuInitializer : IInitializable
    {
        private readonly MenuStateMachine _menuStateMachine;
        private readonly LoadingScreenController _loadingScreenController;

        public MenuInitializer(
            MenuStateMachine menuStateMachine,
            LoadingScreenController loadingScreenController)
        {
            _menuStateMachine = menuStateMachine;
            _loadingScreenController = loadingScreenController;
        }

        public async UniTask InitAsync(CancellationToken ct)
        {
            await _menuStateMachine.SetStateAsync<MainMenuState>(ct);
            _loadingScreenController.Hide();
        }
    }
}