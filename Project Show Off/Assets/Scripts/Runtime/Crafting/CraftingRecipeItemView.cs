using System;
using Runtime.Data;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class CraftingRecipeItemView : MonoBehaviour, IEventSubscriber {
        [Header("Settings"), SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color invalidColor = Color.red;
        [Header("References"), SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;
        

        private ItemStack itemStack;
        private IDisposable inventoryUpdateUnsubscribeToken;
        
        private void Start() {
            inventoryUpdateUnsubscribeToken = EventQueue.Subscribe(this, EventType.InventoryUpdate);
        }

        private void OnDestroy() {
            inventoryUpdateUnsubscribeToken?.Dispose();
        }

        public void Build(ItemStack itemStack, Inventory materialInventory) {
            this.itemStack = itemStack;
            icon.sprite = itemStack.Item.ItemSprite;
            nameText.text = itemStack.Item.ItemName;
            UpdateItemCount(materialInventory.GetItemCount(itemStack.Item));
        }

        private void UpdateItemCount(int itemCount) {
            countText.text = $"{itemCount}/{itemStack.Count}";
            if (itemCount >= itemStack.Count) countText.color = nameText.color = normalColor;
            else countText.color = nameText.color = invalidColor;
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

        private void OnInventoryUpdateEvent(MaterialInventoryUpdateEvent inventoryUpdateEvent) {
            var itemCount = inventoryUpdateEvent.Inventory.GetItemCount(itemStack.Item);
            UpdateItemCount(itemCount);
        }
    }
}