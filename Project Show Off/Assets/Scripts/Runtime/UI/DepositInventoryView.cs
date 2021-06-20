using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class DepositInventoryView : MonoBehaviour, IEventSubscriber {
        [Header("References"), SerializeField] private RectTransform itemViewsContainer;
        [SerializeField] private DepositItemView itemViewPrefab;
        
        private Dictionary<ItemStack, DepositItemView> items;
        private IDisposable depositInventoryUpdateUnsubscribeToken;
        
        private void Start() {
            depositInventoryUpdateUnsubscribeToken = this.Subscribe(EventType.DepositInventoryUpdate);
            items = new Dictionary<ItemStack, DepositItemView>(ItemStack.Comparer);
        }
        
        private void OnDestroy() {
            depositInventoryUpdateUnsubscribeToken?.Dispose();
        }
        
        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case DepositInventoryUpdateEvent inventoryUpdateEvent:
                    OnInventoryUpdateEvent(inventoryUpdateEvent);
                    return false;
                default:
                    return false;
            }
        }
        
        private void OnInventoryUpdateEvent(DepositInventoryUpdateEvent inventoryUpdateEvent) {
            // Update count
            foreach (var itemStack in inventoryUpdateEvent.Inventory) {
                if (!items.ContainsKey(itemStack)) {
                    var itemView = Instantiate(itemViewPrefab, itemViewsContainer);
                    itemView.Build(itemStack);
                    items.Add(itemStack, itemView);
                } else {
                    items[itemStack].UpdateItemCount(itemStack.Mass);
                }
            }
        }
    }
}