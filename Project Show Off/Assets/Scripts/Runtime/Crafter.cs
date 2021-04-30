using System.Collections.Generic;
using Runtime.Data;
using UnityEngine;

namespace Runtime {
    public class Crafter : MonoBehaviour {
        [SerializeField] private List<CraftingRecipe> availableRecipes;
        
        public IReadOnlyList<CraftingRecipe> Recipes => availableRecipes.AsReadOnly();
        
        public bool CanCraft(CraftingRecipe recipe, Inventory inventory) {
            foreach (var input in recipe.Inputs) {
                var itemCount = inventory.GetItemCount(input.Item);
                if (itemCount < input.Count) return false;
            }

            return true;
        }

        public void Craft(CraftingRecipe recipe, Inventory source, Inventory target) {
            foreach (var input in recipe.Inputs) {
                source.RemoveItem(input);
            }
            
            foreach (var output in recipe.Outputs) {
                source.Add(output);
            }
        }
    }
}