using Cinemachine;
using CutTwice.Core.GameStates;

namespace CutTwice.Menu
{
    // Per-stage camera reference. TState (the stage's own IMenuState type) makes every
    // registration a distinct closed generic Type, so each stage's composition root can
    // register its own camera into DI independently, whenever that camera actually exists -
    // no shared pre-populated list, no dependency on registration order.
    public class StageCamera<TState> where TState : IMenuState
    {
        public readonly CinemachineVirtualCamera Camera;

        public StageCamera(CinemachineVirtualCamera camera)
        {
            Camera = camera;
        }
    }
}
