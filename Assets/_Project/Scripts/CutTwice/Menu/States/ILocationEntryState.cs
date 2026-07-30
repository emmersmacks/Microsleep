using CutTwice.Core.GameStates;

namespace CutTwice.Menu.States
{
    // Implemented by exactly one concrete IMenuState per session (ShopMenuState, TowerMenuState, ...) —
    // the choice is made by MenuCompositionRoot.PreCompose when it loads the location prefab by presetId.
    public interface ILocationEntryState : IMenuState
    {
    }
}
