using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class InventoryView : MonoBehaviour, IEventSubscriber {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private RectTransform inventoryItemContainer;
        [SerializeField] private InventoryMaterialView materialViewPrefab;
    
        private float screenUnitsPerMassUnit;
        private List<TrashCategory> trashCategories;
        private Dictionary<TrashCategory, InventoryMaterialView> itemDictionary;
        private List<InventoryMaterialView> items;
        private IDisposable inventoryUpdateEventUnsubscribeToken;

        private void Awake() {
            trashCategories = new List<TrashCategory>(Resources.LoadAll<TrashCategory>("Trash Materials"));
            
            var inventoryScreenSize = ((RectTransform) transform).sizeDelta.y;
            screenUnitsPerMassUnit = inventoryScreenSize / playerInventory.MaximumCarryMass;
            items = new List<InventoryMaterialView>();
            itemDictionary = new Dictionary<TrashCategory, InventoryMaterialView>();
            inventoryUpdateEventUnsubscribeToken = this.Subscribe(EventType.InventoryUpdate);
        }

        private void Start() {
            foreach (var trashCategory in trashCategories) {
                CreateInventoryImage(trashCategory);
            }

            foreach (var itemStack in playerInventory.MaterialInventory) {
                UpdateFillAmount(itemStack);
            }
        }

        private void OnDestroy() {
            inventoryUpdateEventUnsubscribeToken.Dispose();
        }
    
        private void CreateInventoryImage(TrashCategory trashCategory) {
            var materialView = Instantiate(materialViewPrefab, inventoryItemContainer);
            materialView.gameObject.name = $"MaterialView_{items.Count}";
            materialView.LoadUI(trashCategory);
            // debug: remove when fixed
            materialView.SetTransitionEnabled(false);
            itemDictionary[trashCategory] = materialView;
            items.Add(materialView);

            // debug: uncomment when fixed
            /*// update transition image of previous one to match this color
            if (items.Count > 1) {
                var previous = items[items.Count - 2];
                previous.LoadTransition(trashCategory);
            }*/
        }

        private void UpdateFillAmount(ItemStack itemStack) {
            var screenSize = itemStack.Mass * screenUnitsPerMassUnit;
            var materialView = itemDictionary[itemStack.TrashCategory];
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
                if (!itemDictionary.ContainsKey(itemStack.TrashCategory)) CreateInventoryImage(itemStack.TrashCategory);
                UpdateFillAmount(itemStack);
            }
        }
    }
}

