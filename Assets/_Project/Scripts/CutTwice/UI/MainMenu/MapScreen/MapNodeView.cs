using System;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.Gameplay.Runtime.Map.Nodes;
using CutTwice.Gameplay.Runtime.Map.Presets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapNodeView : MonoBehaviour
    {
        public Image Icon;
        public TMP_Text NameLabel;
        public Button Button;
        public GameObject HighlightImage;
        public GameObject CrossroadMarker;
        public GameObject LocationMarker;

        public MapNode Node { get; private set; }
        public event Action<MapNodeView> Clicked;

        private void Awake()
        {
            Button.onClick.AddListener(() => Clicked?.Invoke(this));
        }

        public void Bind(MapNode node)
        {
            Node = node;

            var preset = node switch
            {
                LocationNode location => (NodePreset)location.Preset,
                CrossroadNode crossroad => crossroad.Preset,
                _ => null
            };

            if (Icon != null)
            {
                Icon.sprite = preset != null ? preset.Icon : null;
            }

            if (NameLabel != null)
            {
                NameLabel.text = preset != null ? preset.DisplayName : string.Empty;
            }

            if (CrossroadMarker != null)
            {
                CrossroadMarker.SetActive(node is CrossroadNode);
            }

            if (LocationMarker != null)
            {
                LocationMarker.SetActive(node is LocationNode);
            }

            SetHighlighted(false);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            if (HighlightImage != null)
            {
                HighlightImage.SetActive(isHighlighted);
            }
        }
    }
}
