using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class InventoryView : MonoBehaviour, IEventSubscriber {
        [Header("References"), SerializeField] private RectTransform itemViewsContainer;
        [SerializeField] private InventoryItemView itemViewPrefab;
        
        private Dictionary<ItemStack, InventoryItemView> items;
        private IDisposable inventoryUpdateUnsubscribeToken;
        
        private void Start() {
            inventoryUpdateUnsubscribeToken = EventQueue.Subscribe(this, EventType.InventoryUpdate);
            items = new Dictionary<ItemStack, InventoryItemView>(ItemStack.Comparer);
        }
        
        private void OnDestroy() {
            inventoryUpdateUnsubscribeToken?.Dispose();
        }
        
        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case MaterialInventoryUpdateEvent inventoryUpdateEvent:
                    OnInventoryUpdateEvent(inventoryUpdateEvent);
                    return false;
                default:
                    return false;
            }
        }
        
        private void OnInventoryUpdateEvent(InventoryUpdateEvent inventoryUpdateEvent) {
            // Update count
            foreach (var itemStack in inventoryUpdateEvent.Inventory) {
                if (!items.ContainsKey(itemStack)) {
                    var itemView = Instantiate(itemViewPrefab, itemViewsContainer);
                    itemView.Build(itemStack);
                    items.Add(itemStack, itemView);
                } else {
                    items[itemStack].UpdateItemCount(itemStack.Count);
                }
            }
            
            // Remove empty items
            foreach (var itemStack in items.Keys.ToList()) {
                if (inventoryUpdateEvent.Inventory.GetItemCount(itemStack.Item) > 0) continue;
                
                Destroy(items[itemStack]);
                items.Remove(itemStack);
            }
        }
    }
}