using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.Initialization;
using CutTwice.Menu.GlobalStates;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CutTwice.App
{
    public class AppBootstrap : Bootstrap
    {
        protected override CompositionRoot CreateCompositionRoot()
        {
            return new AppCompositionRoot();
        }

        protected override async UniTask InitAsync(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                await GlobalStateMachine.Instance.SetStateAsync<MainMenuState>(destroyCancellationToken);
            }
        }
    }
}