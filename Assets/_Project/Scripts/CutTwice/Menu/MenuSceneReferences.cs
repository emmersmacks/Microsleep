using System;
using Cinemachine;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.MapCards;
using CutTwice.UI.MainMenu.MapScreen;
using CutTwice.UI.MainMenu.Menu;
using CutTwice.UI.MainMenu.SelectLevel;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Menu
{
    [Serializable]
    public class MenuSceneReferences
    {
        public SelectMapWindowView SelectMapWindowView;
        public SelectLevelWindowView SelectLevelWindowView;
        public CreditsWindowView CreditsWindow;
        public LeaderboardWindowView LeaderboardWindow;
        public MenuWindowView MenuWindow;
        public CinemachineBrain CinemachineBrain;
        public CinemachineVirtualCamera MainMenuCamera;
        public CinemachineVirtualCamera SelectLevelCamera;
        public CinemachineVirtualCamera SelectMapCamera;
        public MapCardSlotsView MapCardSlots;
        public MapWindowView MapWindowView;
    }
}