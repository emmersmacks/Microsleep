using System;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    [Serializable]
    public class RoadExitPath
    {
        public Transform exitPoint;
        public Transform[] controlPoints;
    }

    public class RoadSegmentPresenter : MonoBehaviour
    {
        public float width = 30f;
        public Transform entryPoint;
        public RoadExitPath[] exits;

        [SerializeField] private int activeExitIndex = 0;

        public int ActiveExitIndex => activeExitIndex;

        public RoadExitPath ActiveExit =>
            (activeExitIndex >= 0 && exits != null && activeExitIndex < exits.Length) ? exits[activeExitIndex] : null;

        public Transform ActiveExitPoint => ActiveExit?.exitPoint;

        public void SetActiveExitPoint(int index)
        {
            if (exits == null || index < 0 || index >= exits.Length)
            {
                Debug.LogWarning($"RoadSegmentPresenter: invalid exit index {index}.");
                return;
            }

            activeExitIndex = index;
        }
    }
}
