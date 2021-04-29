using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Data {
    [Serializable]
    public class Inventory {
        [SerializeField] private List<ItemStack> items;

        public Inventory() {
            items = new List<ItemStack>();
        }

        public void AddItem(Item item, int count) {
            if(count <= 0) return;
            var itemStack = GetItemStack(item);
            
            if (itemStack == null) {
                itemStack = new ItemStack(item, 0);
                items.Add(itemStack);
            }

            itemStack.Count += count;
        }

        public void AddItem(ItemStack itemStack) => AddItem(itemStack.Item, itemStack.Count);

        public void RemoveItem(Item item, int count) {
            if(count <= 0) return;
            
            var itemStack = GetItemStack(item);
            var inventoryCount = itemStack == null ? 0 : itemStack.Count;
            if (itemStack == null || inventoryCount < count) throw new Exception("Attempting to remove more items than inventory contains");

            itemStack.Count -= count;
            if (itemStack.Count <= 0) {
                items.Remove(itemStack);
            }
        }

        public void RemoveItem(ItemStack itemStack) => RemoveItem(itemStack.Item, itemStack.Count);

        public int GetItemCount(Item item) {
            var itemStack = GetItemStack(item);
            return itemStack == null ? 0 : itemStack.Count;
        }

        private ItemStack GetItemStack(Item item) {
            return items.FirstOrDefault(stack => stack.Item == item);
        }
    }
}