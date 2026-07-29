using System.Collections.Generic;

namespace CutTwice.Gameplay.Runtime.Map
{
    public class PlayerMapsService
    {
        private readonly List<MapDefinition> _ownedMaps;

        public PlayerMapsService(MapDefinition debugDefaultMap)
        {
            _ownedMaps = new List<MapDefinition>();
            if (debugDefaultMap != null)
            {
                _ownedMaps.Add(debugDefaultMap);
            }
        }

        public IReadOnlyList<MapDefinition> GetOwnedMaps() => _ownedMaps;
    }
}
