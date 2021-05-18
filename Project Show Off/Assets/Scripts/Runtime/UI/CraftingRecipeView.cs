using System;
using Runtime.Data;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class CraftingRecipeView : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private float uncraftableCanvasAlpha = 0.8f;
        [SerializeField] private float uncraftableOverlayAlpha = 0.2f;
        [SerializeField] private float uncraftableScale = 0.975f;
        [Header("References")]
        [SerializeField] private Image resultSprite;
        [SerializeField] private TextMeshProUGUI resultCount;
        [SerializeField] private TextMeshProUGUI resultName;
        [SerializeField] private RectTransform ingredientsContainer;
        [SerializeField] private CraftingRecipeItemView itemPrefab;
        [SerializeField] private Button craftButton;
        [Space, SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image overlayImage;

        private bool canCraft;
        private CraftingRecipe recipe;
        private Inventory materialInventory;
        private IDisposable inventoryUpdateUnsubscribeToken;
        
        private void Start() {
            inventoryUpdateUnsubscribeToken = EventQueue.Subscribe(this, EventType.InventoryUpdate);
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }

        private void OnDestroy() {
            inventoryUpdateUnsubscribeToken?.Dispose();
        }

        public void Build( CraftingRecipe recipe, Inventory materialInventory) {
            this.recipe = recipe;
            this.materialInventory = materialInventory;
            
            // TODO!: Use object pooling
            foreach (var recipeIngredient in recipe.Ingredients) {
                var recipeBuilder = Instantiate(itemPrefab, ingredientsContainer);
                recipeBuilder.Build(recipeIngredient, materialInventory);
            }

            UpdateCraftableState();

            resultSprite.sprite = recipe.Result.TrashCategory.CategorySprite;
            resultName.text = recipe.Result.TrashCategory.CategoryName;
            resultCount.text = $"{recipe.Result.Mass:F2} <b>MU</b>";
        }

        private void OnCraftButtonClicked() {
            EventQueue.QueueEvent(new CraftRequestEvent(this, recipe));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case MaterialInventoryUpdateEvent inventoryUpdateEvent: {
                    OnInventoryUpdateEvent(inventoryUpdateEvent);
                    return false;
                }
                default: return false;
            }
        }

        private void OnInventoryUpdateEvent(InventoryUpdateEvent inventoryUpdateEvent) {
            materialInventory = inventoryUpdateEvent.Inventory;
            UpdateCraftableState();
        }

        private void UpdateCraftableState() {
            canCraft = true;
            foreach (var itemStack in recipe.Ingredients) {
                if (materialInventory.GetTrashCategoryMass(itemStack.TrashCategory) >= itemStack.Mass) continue;
                
                canCraft = false;
                break;
            }

            craftButton.interactable = canCraft;
            
            if (canCraft) {
                transform.localScale = new Vector3(1, 1, 1);
                canvasGroup.alpha = 1.0f;
                
                var overlayImageColor = overlayImage.color;
                overlayImageColor.a = 0.0f;
                overlayImage.color = overlayImageColor;
            } else {
                transform.localScale = new Vector3(uncraftableScale, uncraftableScale, 1);
                canvasGroup.alpha = uncraftableCanvasAlpha;
                
                var overlayImageColor = overlayImage.color;
                overlayImageColor.a = uncraftableOverlayAlpha;
                overlayImage.color = overlayImageColor;
            }
        }
    }
}