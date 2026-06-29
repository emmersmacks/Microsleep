using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.StaticNames;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CutTwice.Gameplay.GlobalStates
{
    public class GameState : IGlobalState
    {
        public UniTask EnterAsync(IStateMachine stateMachine, CancellationToken ct)
        {
            SceneManager.LoadScene(SceneNames.Game);
            return UniTask.CompletedTask;
        }

        public void Exit()
        {
            
        }
    }
}