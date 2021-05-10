using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Data/Crafting Recipe", order = 0)]
    public class CraftingRecipe : ScriptableObject {
        [SerializeField] private Inventory ingredients;
        [SerializeField] private ItemStack result;

        public Inventory Ingredients => ingredients;
        public ItemStack Result => result;
    }
}