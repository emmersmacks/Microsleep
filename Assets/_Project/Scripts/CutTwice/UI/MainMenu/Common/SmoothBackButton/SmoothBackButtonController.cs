using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.MainMenu.SelectLevel.SmoothBackButton
{
    public class SmoothBackButtonController : WindowControllerBase<SmoothBackButtonView>, IInitializable
    {
        private readonly MenuStateMachine _menuStateMachine;
        private CancellationToken _cancellationToken;

        public SmoothBackButtonController(SmoothBackButtonView view, MenuStateMachine menuStateMachine) : base(view)
        {
            _menuStateMachine = menuStateMachine;
            view.BackButton.onClick.AddListener(OnBackButtonClicked);
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            return UniTask.CompletedTask;
        }

        private void OnBackButtonClicked()
        {
            _menuStateMachine.GoBackAsync(_cancellationToken).Forget(Debug.LogException);
        }
    }
}