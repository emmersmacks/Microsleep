using UnityEngine;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapConnectionView : MonoBehaviour
    {
        public RectTransform LineRect;
        public GameObject HighlightImage;

        public void SetEndpoints(Vector2 from, Vector2 to)
        {
            var delta = to - from;

            LineRect.anchoredPosition = from;
            LineRect.sizeDelta = new Vector2(delta.magnitude, LineRect.sizeDelta.y);
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
