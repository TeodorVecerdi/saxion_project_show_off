using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data {
    [Serializable]
    public class ItemStack : IEquatable<ItemStack> {
        [SerializeField] private TrashCategory trashCategory;
        [SerializeField] private float mass;
        
        public TrashCategory TrashCategory => trashCategory;
        
        public float Mass {
            get => mass;
            set => mass = value;
        }

        public ItemStack(TrashCategory trashCategory, float mass) {
            this.trashCategory = trashCategory;
            this.mass = mass;
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
            return trashCategory.GetHashCode();
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
                return Equals(x.trashCategory, y.trashCategory);
            }

            public int GetHashCode(ItemStack obj) {
                return obj.trashCategory.GetHashCode();
            }
        }
    }
}