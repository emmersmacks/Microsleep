using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CutTwice.Core.EventBus;

namespace CutTwice.Menu
{
    [Serializable]
    public class CameraSwitchContext
    {
        [Serializable]
        public struct CameraData
        {
            public MenuCameraType CameraType;
            public CinemachineVirtualCamera Camera;
        }
        
        public CinemachineBrain CinemachineBrain;
        public List<CameraData> Cameras;
        
        public CinemachineVirtualCamera GetCamera(MenuCameraType cameraType)
        {
            return Cameras.FirstOrDefault(x => x.CameraType == cameraType).Camera;
        }
    }
}