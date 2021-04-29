using Editor.Editors;
using Runtime.Data;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.Utils.Components {
    public class ItemStackElement : VisualElement {
        public ItemStack ItemStack => itemStack;
        
        private readonly CraftingRecipeEditor owner;
        private readonly ItemStack itemStack;

        private readonly ObjectField itemObjectField;
        private readonly IntegerField countField;

        public ItemStackElement(CraftingRecipeEditor owner, ItemStack itemStack) {
            this.owner = owner;
            this.itemStack = itemStack;

            itemObjectField = new ObjectField("Item") {name = "ItemField", objectType = typeof(Item)};
            countField = new IntegerField("Count") {name = "CountField"};
            
            var remove = new Button(OnRemove) {name = "RemoveButton", text = "", tooltip = "Remove"};
            
            Add(itemObjectField);
            Add(countField);
            Add(remove);
        }

        private void OnRemove() {
            owner.RemoveItemStack(this);
        }
    }
}