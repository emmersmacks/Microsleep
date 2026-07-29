using System;
using UnityEngine;

namespace CutTwice.UI.MainMenu.MapCards
{
    [RequireComponent(typeof(Collider))]
    public class MapCardView : MonoBehaviour
    {
        public Light SpotLight;
        public AudioSource AudioSource;
        public AudioClip ClickSound;

        public event Action<MapCardView> Clicked;

        private void OnMouseDown()
        {
            Clicked?.Invoke(this);
        }
    }
}
