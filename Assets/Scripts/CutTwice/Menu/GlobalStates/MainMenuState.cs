using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.StaticNames;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CutTwice.Menu.GlobalStates
{
    public class MainMenuState : IGlobalState
    {
        public UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
            return UniTask.CompletedTask;
        }

        public void Exit() { }
    }
}