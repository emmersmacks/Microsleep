using UnityEngine;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapConnectionView : MonoBehaviour
    {
        public RectTransform LineRect;
        public GameObject HighlightImage;

        [Tooltip("Distance to shrink the line at each end, keeping it clear of the node view it connects to.")]
        public float EndpointPadding;

        public void SetEndpoints(Vector2 from, Vector2 to)
        {
            var delta = to - from;
            var length = delta.magnitude;
            var direction = length > Mathf.Epsilon ? delta / length : Vector2.right;

            var padding = Mathf.Min(EndpointPadding, length * 0.5f);
            var paddedFrom = from + direction * padding;
            var paddedLength = Mathf.Max(0f, length - padding * 2f);

            LineRect.anchoredPosition = paddedFrom;
            LineRect.sizeDelta = new Vector2(paddedLength, LineRect.sizeDelta.y);
            LineRect.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
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
