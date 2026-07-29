using System.Threading;
using CutTwice.Core.GameStates;
using CutTwice.Gameplay.GlobalStates;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Menu.GlobalStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.Gameplay.Modes
{
    public class AdventureFlowService
    {
        private const float AdventureLegDistanceMeters = 100f;

        private readonly MapProgressService _mapProgressService;
        private readonly GameModeContext _gameModeContext;
        private readonly GlobalStateMachine _globalStateMachine;

        public AdventureFlowService(MapProgressService mapProgressService, GameModeContext gameModeContext, GlobalStateMachine globalStateMachine)
        {
            _mapProgressService = mapProgressService;
            _gameModeContext = gameModeContext;
            _globalStateMachine = globalStateMachine;
        }

        public bool TryStartAdventure(CancellationToken ct)
        {
            if (!_mapProgressService.HasSelectedMap)
            {
                return false;
            }

            _mapProgressService.ActivateSelectedMap();

            if (_mapProgressService.GetNextTargetLocation() == null)
            {
                return false;
            }

            StartNextLeg(ct);
            return true;
        }

        public bool TryHandleRunCompleted(CancellationToken ct)
        {
            if (!_mapProgressService.IsMapActive)
            {
                return false;
            }

            _mapProgressService.MarkLocationCompleted();
            _globalStateMachine.SetStateAsync<GlobalLocationState>(ct).Forget(Debug.LogException);
            return true;
        }

        public void HandleRunFailed()
        {
            if (_mapProgressService.IsMapActive)
            {
                _mapProgressService.ResetProgress();
            }
        }

        public void ContinueFromLocation(CancellationToken ct)
        {
            if (!_mapProgressService.IsMapActive)
            {
                Debug.LogWarning("AdventureFlowService.ContinueFromLocation called with no active map.");
                return;
            }

            if (_mapProgressService.IsCurrentPositionDeadEnd())
            {
                _mapProgressService.ResetProgress();
                _globalStateMachine.SetStateAsync<GlobalMainMenuState>(ct).Forget(Debug.LogException);
                return;
            }

            if (_mapProgressService.GetNextTargetLocation() != null)
            {
                StartNextLeg(ct);
                return;
            }

            Debug.LogWarning("AdventureFlowService.ContinueFromLocation called with no target selected and current node has multiple exits — caller must AddTargetLocation first.");
        }

        private void StartNextLeg(CancellationToken ct)
        {
            _gameModeContext.SetMode(new DistanceModeConfig(AdventureLegDistanceMeters));
            _globalStateMachine.SetStateAsync<GlobalGameState>(ct).Forget(Debug.LogException);
        }
    }
}
