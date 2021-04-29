using Runtime.Data;
using UnityEngine;

namespace Runtime {
    public class PlayerInventory : MonoBehaviour {
        [SerializeField] private Inventory materialInventory;
        [SerializeField] private Inventory placeableInventory;

        public void AddMaterial(ItemStack itemStack) {
            materialInventory.AddItem(itemStack);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Crafter")) {
                // Todo: open crafter UI, load recipes, etc
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Crafter")) {
                // Todo: close crafter UI
            }
        }
    }
}