using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Data/Crafting Recipe", order = 0)]
    public class CraftingRecipe : ScriptableObject {
        [SerializeField] private List<ItemStack> inputs;
        [SerializeField] private List<ItemStack> outputs;

        public IReadOnlyList<ItemStack> Inputs => inputs.AsReadOnly();
        public IReadOnlyList<ItemStack> Outputs => outputs.AsReadOnly();
    }
}