using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    public class InfiniteRoadPresenter : MonoBehaviour
    {
        public Transform[] roadSegments;     // Сегменты дороги
        public Transform finalSegment;
        public float moveSpeed = 10f;        // Скорость движения назад
        public float segmentLength = 30f;    // Длина одного сегмента
        public float recycleZ = -50f;        // Когда перемещать назад
    }
}