using Runtime.Data;
using UnityEngine;

namespace Runtime {
    public class Pickup : MonoBehaviour {
        [SerializeField] private ItemStack itemStack;
        
        public ItemStack ItemStack => itemStack;
    }
}