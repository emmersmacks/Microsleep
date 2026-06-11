using CutTwice.Core.Initialization;

namespace CutTwice.Menu
{
    public class MenuBootstrap : Bootstrap
    {
        public MenuSceneReferences SceneReferences;
        
        protected override CompositionRoot CreateCompositionRoot()
        {
            return new MenuCompositionRoot(SceneReferences);
        }
    }
}