using System.Collections.Generic;
using System.Linq;
using CutTwice.Core.Lifecycle;
using UnityEngine;

namespace CutTwice.Gameplay.Runtime.Player.Components
{
    public class PlayerInputController : ITickable
    {
        private readonly Camera _camera;
        public Vector2 MoveVector;
        public bool Enabled;
        
        public List<RaycastHit> Hits;

        public PlayerInputController(Camera camera)
        {
            _camera = camera;
            Hits = new List<RaycastHit>(10);
        }

        public void Tick()
        {
            MoveVector = Vector2.zero;
            Hits.Clear();
            
            if (!Enabled)
            {
                return;
            }

            HandleMobileInput();
            HandleDesktopInput();
        }

        private void HandleMobileInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    HandleHits(touch.position);
                    
                    if (IsPointerOverInteractable())
                        return;
                    
                    HandleInput(touch.position);
                }
            }
        }

        private void HandleDesktopInput()
        {
            if (Input.GetMouseButton(0))
            {
                HandleHits(Input.mousePosition);
                
                if (IsPointerOverInteractable())
                    return;

                HandleInput(Input.mousePosition);
            }
        }

        private bool IsPointerOverInteractable()
        {
            var interactableHits = Hits.Where(h => h.transform.gameObject.layer == LayerMask.NameToLayer("Interactable")).ToArray();
            return interactableHits.Length > 0;
        }

        private void HandleHits(Vector3 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            var hits = Physics.RaycastAll(ray);
            Hits.AddRange(hits);
        }

        void HandleInput(Vector2 position)
        {
            if (position.x < Screen.width / 2f)
            {
                MoveVector = Vector2.left;
            }
            else
            {
                MoveVector = Vector2.right;
            }
        }
    }
}