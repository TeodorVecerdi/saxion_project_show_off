using NaughtyAttributes;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;

namespace Runtime {
    public class CrafterView : MonoBehaviour {
        [Header("References")] [SerializeField] private RectTransform recipeContainer;
        [SerializeField] private GameObject mainUI;
        [SerializeField] private CraftingRecipeView recipePrefab;
        [SerializeField, ReadOnly] /*debug:*/ private bool isMenuOpen;

        private Crafter currentCrafter;
        private PlayerInventory playerInventory;

        public bool IsMenuOpen => isMenuOpen;

        public void OpenView(Crafter crafter, PlayerInventory inventory) {
            isMenuOpen = true;
            currentCrafter = crafter;
            playerInventory = inventory;

            UIHintController.Instance.RequestHide(this);
            LookWithMouse.Instance.SetEnabled(false);
            PlayerMovement.Instance.SetEnabled(false);
            mainUI.SetActive(true);
            
            LoadUI();
        }

        private void CloseView() {
            isMenuOpen = false;
            
            UIHintController.Instance.ReleaseHide(this);
            LookWithMouse.Instance.SetEnabled(true);
            PlayerMovement.Instance.SetEnabled(true);
            mainUI.SetActive(false);
            
            // Todo: Implement pooling
            while (recipeContainer.childCount > 0) {
                var child = recipeContainer.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void LoadUI() {
            if (currentCrafter == null || playerInventory == null) {
                Debug.LogError("Attempting to load crafter UI without a crafter or player inventory");
                CloseView();
                return;
            }

            // Todo: Implement pooling
            foreach (var recipe in currentCrafter.Recipes) {
                var recipeView = Instantiate(recipePrefab, recipeContainer);
                recipeView.Build(this, recipe, playerInventory.MaterialInventory);
            }
        }

        public void RequestCraft(CraftingRecipe recipe) {
            if(!playerInventory.MaterialInventory.Contains(recipe.Ingredients)) return;
            playerInventory.MaterialInventory.Remove(recipe.Ingredients);
            playerInventory.MaterialInventory.Add(recipe.Result);
            EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, playerInventory.MaterialInventory));
        }

        public void OnCloseButtonClicked() {
            CloseView();
        }
    }
}