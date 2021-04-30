using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Data;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class InventoryTest : MonoBehaviour {
        public List<Item> AllItems;
        public List<CraftingRecipe> AllRecipes;
        public Inventory Inventory;

        public int RecipeIndex;

        [Button]
        public void AddRandomItem() {
            var item = Rand.ListItem(AllItems);
            var count = Rand.Range(1, 4);
            Inventory.Add(item, count);
        }

        [Button]
        public void RemoveRandomItem() {
            var item = Rand.ListItem(AllItems);
            var count = Rand.Range(1, 4);
            try {
                Inventory.Remove(item, count);
            } catch {
                Debug.Log("Not enough items in inventory");
            }
        }

        [Button]
        public void CanCraftSelectedRecipe() {
            Debug.Log(CanCraftRecipe(AllRecipes[RecipeIndex]) ? "Yes" : "No");
        }

        [Button]
        public void CraftSelectedRecipe() {
            var recipe = AllRecipes[RecipeIndex];
            if(!CanCraftRecipe(recipe)) return;
            
            foreach (var recipeInput in recipe.Ingredients) {
                Inventory.Remove(recipeInput.Item, recipeInput.Count);
            }
            foreach (var recipeOutput in recipe.Results) {
                Inventory.Add(recipeOutput.Item, recipeOutput.Count);
            }
        }

        public bool CanCraftRecipe(CraftingRecipe recipe) {
            foreach (var recipeInput in recipe.Ingredients) {
                if (Inventory.GetItemCount(recipeInput.Item) < recipeInput.Count) {
                    return false;
                }
            }
            return true;
        }
    }
}