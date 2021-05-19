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

        public void Build(ItemStack itemStack, MaterialInventory materialInventory) {
            this.itemStack = itemStack;
            icon.sprite = itemStack.TrashCategory.CategorySprite;
            nameText.text = itemStack.TrashCategory.CategoryName;
            UpdateItemCount(materialInventory.GetTrashCategoryMass(itemStack.TrashCategory));
        }

        private void UpdateItemCount(float mass) {
            countText.text = $"{mass:F1} <b>MU</b>/{itemStack.Mass:F1} <b>MU</b>";
            if (mass >= itemStack.Mass) countText.color = nameText.color = normalColor;
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

        private void OnInventoryUpdateEvent(InventoryUpdateEvent inventoryUpdateEvent) {
            var mass = inventoryUpdateEvent.Inventory.GetTrashCategoryMass(itemStack.TrashCategory);
            UpdateItemCount(mass);
        }
    }
}