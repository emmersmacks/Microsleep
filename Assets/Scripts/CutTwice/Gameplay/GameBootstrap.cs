using CutTwice.Core.Initialization;

namespace CutTwice.Gameplay
{
    public class GameBootstrap : Bootstrap
    {
        public GameSceneReferences SceneReferences;
        
        protected override CompositionRoot CreateCompositionRoot()
        {
            return new GameCompositionRoot(SceneReferences);
        }
    }
}