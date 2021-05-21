using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

public class InventoryView : MonoBehaviour, IEventSubscriber {
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private RectTransform inventoryItemContainer;
    [SerializeField] private List<TrashColorPair> trashColors;
    
    private float screenUnitsPerMassUnit;
    private Dictionary<TrashCategory, Image> items;
    private IDisposable inventoryUpdateEventUnsubscribeToken;

    private void Awake() {
        var inventoryScreenSize = ((RectTransform) transform).sizeDelta.y;
        screenUnitsPerMassUnit = inventoryScreenSize / playerInventory.MaximumCarryMass;
        items = new Dictionary<TrashCategory, Image>();
        inventoryUpdateEventUnsubscribeToken = EventQueue.Subscribe(this, EventType.InventoryUpdate);
    }

    private void Start() {
        foreach (var trashColorPair in trashColors) {
            CreateInventoryImage(new ItemStack(trashColorPair.Category, 0.0f));
        }

        foreach (var itemStack in playerInventory.MaterialInventory) {
            UpdateFillAmount(itemStack);
        }
    }

    private void OnDestroy() {
        inventoryUpdateEventUnsubscribeToken.Dispose();
    }
    
    private void CreateInventoryImage(ItemStack itemStack) {
        var go = new GameObject(itemStack.TrashCategory.CategoryName, typeof(Image));
        go.transform.SetParent(inventoryItemContainer);
        var imageComp = items[itemStack.TrashCategory] = go.GetComponent<Image>();
        
        imageComp.color = trashColors.FirstOrDefault(pair => pair.Category == itemStack.TrashCategory)?.Color ?? Color.magenta;
        
        var sizeDelta = imageComp.rectTransform.sizeDelta;
        sizeDelta.y = 0.0f;
        imageComp.rectTransform.sizeDelta = sizeDelta;
    }

    private void UpdateFillAmount(ItemStack itemStack) {
        var screenSize = itemStack.Mass * screenUnitsPerMassUnit;
        var sizeDelta = items[itemStack.TrashCategory].rectTransform.sizeDelta;
        sizeDelta.y = screenSize;
        items[itemStack.TrashCategory].rectTransform.DOKill(true);
        items[itemStack.TrashCategory].rectTransform.DOSizeDelta(sizeDelta, 0.25f);
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
            if (!items.ContainsKey(itemStack.TrashCategory)) CreateInventoryImage(itemStack);
            UpdateFillAmount(itemStack);
        }
    }
    
    
    [Serializable]
    private class TrashColorPair {
        public TrashCategory Category;
        public Color Color;
    }
}

