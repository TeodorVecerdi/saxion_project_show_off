using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class InventoryView : MonoBehaviour, IEventSubscriber {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private RectTransform inventoryItemContainer;
        [SerializeField] private InventoryMaterialView materialViewPrefab;
    
        private float screenUnitsPerMassUnit;
        private Dictionary<TrashMaterial, InventoryMaterialView> itemDictionary;
        private List<InventoryMaterialView> items;
        private IDisposable inventoryUpdateEventUnsubscribeToken;

        private void Awake() {
            var inventoryScreenSize = ((RectTransform) transform).sizeDelta.y;
            screenUnitsPerMassUnit = inventoryScreenSize / playerInventory.MaximumCarryMass;
            items = new List<InventoryMaterialView>();
            itemDictionary = new Dictionary<TrashMaterial, InventoryMaterialView>();
            inventoryUpdateEventUnsubscribeToken = this.Subscribe(EventType.InventoryUpdate);
        }

        private void Start() {
            foreach (var trashMaterial in ResourcesProvider.TrashMaterials) {
                CreateInventoryImage(trashMaterial);
            }

            foreach (var itemStack in playerInventory.MaterialInventory) {
                UpdateFillAmount(itemStack);
            }
        }

        private void OnDestroy() {
            inventoryUpdateEventUnsubscribeToken.Dispose();
        }
    
        private void CreateInventoryImage(TrashMaterial trashMaterial) {
            var materialView = Instantiate(materialViewPrefab, inventoryItemContainer);
            materialView.gameObject.name = $"MaterialView_{items.Count}";
            materialView.LoadUI(trashMaterial);
            // debug: remove when fixed
            materialView.SetTransitionEnabled(false);
            itemDictionary[trashMaterial] = materialView;
            items.Add(materialView);

            // debug: uncomment when fixed
            /*// update transition image of previous one to match this color
            if (items.Count > 1) {
                var previous = items[items.Count - 2];
                previous.LoadTransition(trashMaterial);
            }*/
        }

        private void UpdateFillAmount(ItemStack itemStack) {
            var screenSize = itemStack.Mass * screenUnitsPerMassUnit;
            var materialView = itemDictionary[itemStack.TrashMaterial];
            materialView.UpdateSize(screenSize);
            
            // debug: uncomment when fixed
            /*var currentIndex = items.IndexOf(materialView);
            if (currentIndex < items.Count - 1) {
                var previous = items[currentIndex + 1];
                previous.SetTransitionEnabled(screenSize > 10.0f);
            }*/
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case MaterialInventoryUpdateEvent inventoryUpdateEvent: {
                    OnInventoryUpdate(inventoryUpdateEvent.Inventory);
                    return false;
                }
                default: return false;
            }
        }

        private void OnInventoryUpdate(MaterialInventory inventory) {
            foreach (var itemStack in inventory) {
                if (!itemDictionary.ContainsKey(itemStack.TrashMaterial)) CreateInventoryImage(itemStack.TrashMaterial);
                UpdateFillAmount(itemStack);
            }
        }
    }
}

