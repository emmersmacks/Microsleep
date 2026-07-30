using Cinemachine;

namespace CutTwice.Menu
{
    public class MenuCameraSwitcher
    {
        private const int ActivePriority = 20;
        private const int InactivePriority = 10;

        private readonly CinemachineBrain _brain;
        private CinemachineVirtualCamera _activeCamera;

        public MenuCameraSwitcher(CinemachineBrain brain)
        {
            _brain = brain;
        }

        public void SwitchTo(CinemachineVirtualCamera camera)
        {
            if (camera == null || camera == _activeCamera)
            {
                return;
            }

            camera.Priority = ActivePriority;
            if (_activeCamera != null)
            {
                _activeCamera.Priority = InactivePriority;
            }

            _activeCamera = camera;
        }

        public void CutBlend()
        {
            if (_brain != null)
            {
                _brain.ActiveBlend = null;
            }
        }
    }
}
