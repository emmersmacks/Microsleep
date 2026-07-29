using System.Collections.Generic;
using System.Threading;
using CutTwice.Core.EventBus;
using CutTwice.Core.Lifecycle;
using CutTwice.Core.RivletUI;
using CutTwice.Gameplay.Runtime.Map;
using CutTwice.UI.MainMenu.MapScreen;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CutTwice.UI.MainMenu.MapCards
{
    public class MapCardSlotsController : IInitializable
    {
        private readonly MapCardSlotsView _view;
        private readonly PlayerMapsService _playerMapsService;
        private readonly MapProgressService _mapProgressService;
        private readonly IEventBus _eventBus;
        private readonly List<MapCardView> _spawnedCards = new();

        private MapCardView _selectedCard;

        public MapCardSlotsController(
            MapCardSlotsView view,
            PlayerMapsService playerMapsService,
            MapProgressService mapProgressService,
            IEventBus eventBus)
        {
            _view = view;
            _playerMapsService = playerMapsService;
            _mapProgressService = mapProgressService;
            _eventBus = eventBus;
        }

        public UniTask InitAsync(CancellationToken ct)
        {
            SpawnCards();
            return UniTask.CompletedTask;
        }

        private void SpawnCards()
        {
            var ownedMaps = _playerMapsService.GetOwnedMaps();
            var slotCount = Mathf.Min(ownedMaps.Count, _view.Slots.Length);

            for (var i = 0; i < slotCount; i++)
            {
                var map = ownedMaps[i];
                var card = Object.Instantiate(_view.CardPrefab, _view.Slots[i]);
                if (card.SpotLight != null)
                {
                    card.SpotLight.enabled = false;
                }

                card.Clicked += _ => OnCardClicked(card, map);
                _spawnedCards.Add(card);
            }
        }

        private void OnCardClicked(MapCardView card, MapDefinition map)
        {
            if (_selectedCard != null && _selectedCard.SpotLight != null)
            {
                _selectedCard.SpotLight.enabled = false;
            }

            if (card.AudioSource != null && card.ClickSound != null)
            {
                card.AudioSource.PlayOneShot(card.ClickSound);
            }

            if (card.SpotLight != null)
            {
                card.SpotLight.enabled = true;
            }

            _selectedCard = card;
            _mapProgressService.SelectMap(map);
            _mapProgressService.ActivateSelectedMap();
            _eventBus.Publish(new PushWindowRequest<MapWindow>());
        }
    }
}
