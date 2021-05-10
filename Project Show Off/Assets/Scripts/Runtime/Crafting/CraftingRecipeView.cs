using Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime {
    public class CraftingRecipeView : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image resultSprite;
        [SerializeField] private TextMeshProUGUI resultCount;
        [SerializeField] private TextMeshProUGUI resultName;
        [Space, SerializeField] private RectTransform ingredientsContainer;
        [SerializeField] private CraftingRecipeItemView itemPrefab;
        
        public void Build(CraftingRecipe recipe, Inventory materialInventory) {
            foreach (var recipeIngredient in recipe.Ingredients) {
                var recipeBuilder = Instantiate(itemPrefab, ingredientsContainer);
                recipeBuilder.Build(recipeIngredient, materialInventory);
            }

            resultSprite.sprite = recipe.Result.Item.ItemSprite;
            resultName.text = recipe.Result.Item.ItemName;
            resultCount.text = $"x{recipe.Result.Count}";
        }
    }
}