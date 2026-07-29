using CutTwice.Core.RivletUI;
using CutTwice.UI.Common.UIBackButton;
using UnityEngine;

namespace CutTwice.UI.MainMenu.MapScreen
{
    public class MapWindowView : WindowViewBase
    {
        public UIBackButtonView BackButtonView;
        public RectTransform ContentRoot;
        public MapNodeView NodeViewPrefab;
        public MapConnectionView ConnectionViewPrefab;
    }
}
