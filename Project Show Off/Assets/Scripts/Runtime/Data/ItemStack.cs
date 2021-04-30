﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Runtime.Data {
    [Serializable]
    public class ItemStack : IEquatable<ItemStack> {
        [SerializeField] private Item item;
        [SerializeField] private int count;
        
        public Item Item => item;
        
        public int Count {
            get => count;
            set => count = value;
        }
        

        public ItemStack(Item item, int count) {
            this.item = item;
            this.count = count;
        }

        public bool Equals(ItemStack other) {
            return Comparer.Equals(this, other);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ItemStack) obj);
        }

        public override int GetHashCode() {
            return item!.GetHashCode();
        }

        public static bool operator ==(ItemStack left, ItemStack right) {
            return Comparer.Equals(left, right);
        }

        public static bool operator !=(ItemStack left, ItemStack right) {
            return !Comparer.Equals(left, right);
        }

        public static readonly IEqualityComparer<ItemStack> Comparer = new EqualityComparer();

        private class EqualityComparer : IEqualityComparer<ItemStack> {
            public bool Equals(ItemStack x, ItemStack y) {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.item, y.item);
            }

            public int GetHashCode(ItemStack obj) {
                return obj!.item!.GetHashCode();
            }
        }
    }
}