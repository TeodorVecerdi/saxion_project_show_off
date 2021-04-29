using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Data {
    public class Inventory {
        private readonly List<ItemStack> items;

        public Inventory() {
            items = new List<ItemStack>();
        }

        public void AddItem(ItemSO item, int count) {
            if(count <= 0) return;
            var itemStack = GetItemStack(item);
            
            if (itemStack == null) {
                itemStack = new ItemStack(item, 0);
                items.Add(itemStack);
            }

            itemStack.Count += count;
        }

        public void RemoveItem(ItemSO item, int count) {
            if(count <= 0) return;
            
            var itemStack = GetItemStack(item);
            var inventoryCount = itemStack == null ? 0 : itemStack.Count;
            if (itemStack == null || inventoryCount < count) throw new Exception("Attempting to remove more items than inventory contains");

            itemStack.Count -= count;
            if (itemStack.Count <= 0) {
                items.Remove(itemStack);
            }
        }

        public int GetItemCount(ItemSO item) {
            var itemStack = GetItemStack(item);
            return itemStack == null ? 0 : itemStack.Count;
        }

        private ItemStack GetItemStack(ItemSO item) {
            return items.FirstOrDefault(stack => stack.Item == item);
        }
    }
}