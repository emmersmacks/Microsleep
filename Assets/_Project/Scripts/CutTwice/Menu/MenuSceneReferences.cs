using System;
using Cinemachine;
using CutTwice.Core.Initialization;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.MapCards;
using CutTwice.UI.MainMenu.Menu;
using CutTwice.UI.MainMenu.SelectLevel;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Menu
{
    [Serializable]
    public class MenuSceneReferences
    {
        public SelectMapWindowView SelectMapWindowView;
        public CompositionRoot Location;
        public SelectLevelWindowView SelectLevelWindowView;
        public CreditsWindowView CreditsWindow;
        public LeaderboardWindowView LeaderboardWindow;
        public MenuWindowView MenuWindow;
        public CameraSwitchContext CameraSwitchContext;
        public MapCardSlotsView MapCardSlots;
    }
}