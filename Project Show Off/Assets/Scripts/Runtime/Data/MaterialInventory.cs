using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Data {
    [Serializable]
    public class MaterialInventory : IEnumerable {
        [SerializeField] private List<ItemStack> contents;
        [SerializeField] private float totalMass;

        public float TotalMass => totalMass;
        
        public MaterialInventory() {
            contents = new List<ItemStack>();
        }

        public void Add(TrashCategory trashCategory, float mass) {
            if(mass <= 0) return;
            var itemStack = GetItemStack(trashCategory);
            
            if (itemStack == null) {
                itemStack = new ItemStack(trashCategory, 0);
                contents.Add(itemStack);
            }

            totalMass += mass;
            itemStack.Mass += mass;
        }

        public void Add(ItemStack itemStack) => Add(itemStack.TrashCategory, itemStack.Mass);

        public void Add(MaterialInventory inventory) {
            foreach (var itemStack in inventory) {
                Add(itemStack);
            }
        }

        public void Remove(TrashCategory trashCategory, float mass) {
            if(mass <= 0) return;
            
            var itemStack = GetItemStack(trashCategory);
            var inventoryMass = itemStack == null ? 0.0f : itemStack.Mass;
            if (itemStack == null || inventoryMass < mass) throw new Exception("Attempting to remove more items than inventory contains");

            itemStack.Mass -= mass;
            totalMass -= mass;
            if (itemStack.Mass <= 0) {
                itemStack.Mass = 0.0f;
            }
        }

        public void Remove(ItemStack itemStack) => Remove(itemStack.TrashCategory, itemStack.Mass);

        public void Remove(MaterialInventory inventory) {
            foreach (var itemStack in inventory) {
                Remove(itemStack);
            }
        }

        public float GetTrashCategoryMass(TrashCategory trashCategory) {
            var itemStack = GetItemStack(trashCategory);
            return itemStack == null ? 0 : itemStack.Mass;
        }

        private ItemStack GetItemStack(TrashCategory trashCategory) {
            return contents.FirstOrDefault(stack => stack.TrashCategory == trashCategory);
        }

        public bool Contains(MaterialInventory other) {
            foreach (var itemStack in other) {
                var selfStack = GetItemStack(itemStack.TrashCategory);
                if (selfStack == null || selfStack.Mass < itemStack.Mass) 
                    return false;
            }

            return true;
        }

        public void Clear() {
            foreach (var itemStack in contents) {
                itemStack.Mass = 0.0f;
            }

            totalMass = 0.0f;
        }

        public IEnumerator<ItemStack> GetEnumerator() {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}