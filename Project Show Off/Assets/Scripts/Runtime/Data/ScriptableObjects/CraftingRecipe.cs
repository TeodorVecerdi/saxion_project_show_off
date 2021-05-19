using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Data/Crafting Recipe", order = 0)]
    public class CraftingRecipe : ScriptableObject {
        [SerializeField] private MaterialInventory ingredients;
        [SerializeField] private ItemStack result;

        public MaterialInventory Ingredients => ingredients;
        public ItemStack Result => result;
    }
}