using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Data {
    [Serializable]
    public class MaterialInventory : IEnumerable {
        [SerializeField] private List<ItemStack> contents;

        private float totalMass;
        public float TotalMass => totalMass;
        
        public MaterialInventory() {
            contents = new List<ItemStack>();
        }

        public void Add(TrashMaterial trashMaterial, float mass) {
            if(mass <= 0) return;
            var itemStack = GetItemStack(trashMaterial);
            
            if (itemStack == null) {
                itemStack = new ItemStack(trashMaterial, 0);
                contents.Add(itemStack);
            }

            totalMass += mass;
            itemStack.Mass += mass;
        }

        public void Add(ItemStack itemStack) => Add(itemStack.TrashMaterial, itemStack.Mass);

        public void Add(MaterialInventory inventory) {
            foreach (var itemStack in inventory) {
                Add(itemStack);
            }
        }

        public void Remove(TrashMaterial trashMaterial, float mass) {
            if(mass <= 0) return;
            
            var itemStack = GetItemStack(trashMaterial);
            var inventoryMass = itemStack == null ? 0.0f : itemStack.Mass;
            if (itemStack == null || inventoryMass < mass) throw new Exception("Attempting to remove more items than inventory contains");

            itemStack.Mass -= mass;
            totalMass -= mass;
            if (itemStack.Mass <= 0) {
                itemStack.Mass = 0.0f;
            }
        }

        public void Remove(ItemStack itemStack) => Remove(itemStack.TrashMaterial, itemStack.Mass);

        public void Remove(MaterialInventory inventory) {
            foreach (var itemStack in inventory) {
                Remove(itemStack);
            }
        }

        public float GetTrashMaterialMass(TrashMaterial trashMaterial) {
            var itemStack = GetItemStack(trashMaterial);
            return itemStack == null ? 0 : itemStack.Mass;
        }

        private ItemStack GetItemStack(TrashMaterial trashMaterial) {
            return contents.FirstOrDefault(stack => stack.TrashMaterial == trashMaterial);
        }

        public bool Contains(MaterialInventory other) {
            foreach (var itemStack in other) {
                var selfStack = GetItemStack(itemStack.TrashMaterial);
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

        public ItemStack First(Func<ItemStack, bool> predicate) {
            return contents.First(predicate);
        }
        
        public ItemStack FirstOrDefault(Func<ItemStack, bool> predicate) {
            return contents.FirstOrDefault(predicate);
        }

        public IEnumerator<ItemStack> GetEnumerator() {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}