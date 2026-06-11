using System;
using CutTwice.UI.MainMenu.Credits;
using CutTwice.UI.MainMenu.Leaderboard;
using CutTwice.UI.MainMenu.Menu;
using CutTwice.UI.MainMenu.Shop;

namespace CutTwice.Menu
{
    [Serializable]
    public class MenuSceneReferences
    {
        public CreditsWindowView CreditsWindow;
        public LeaderboardWindowView LeaderboardWindow;
        public MenuWindowView MenuWindow;
        public ShopWindowView ShopWindow;
    }
}