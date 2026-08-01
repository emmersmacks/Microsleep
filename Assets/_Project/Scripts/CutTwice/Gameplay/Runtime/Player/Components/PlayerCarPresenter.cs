using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    public class PlayerCarPresenter : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] public float moveSpeed = 10f;

        [Header("Bounds")]
        [SerializeField] public float minX = -5f;
        [SerializeField] public float maxX = 5f;

        public float MidX => (minX + maxX) * 0.5f;
        public bool IsOnLeftSide => transform.position.x < MidX;

        [Header("References")]
        public Transform steeringWheel;

        [Header("Settings")]
        public float maxAngle = 45f;

        public float smoothSpeed = 10f;

        public Vector3 rotationAxis = Vector3.forward;
    }
}