using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Data/Crafting Recipe", order = 0)]
    public class CraftingRecipe : ScriptableObject {
        [SerializeField] private List<ItemStack> inputs;
        [SerializeField] private List<ItemStack> outputs;

        public List<ItemStack> Inputs => inputs;
        public List<ItemStack> Outputs => outputs;
    }
}