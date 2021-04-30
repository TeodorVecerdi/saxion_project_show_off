using UnityEngine;

namespace Runtime {
    public class Crafter : MonoBehaviour {
        [SerializeField] private CrafterView crafterView;
        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player")) return;
            crafterView.OpenView();
        }

        private void OnTriggerExit(Collider other) {
            if(!other.CompareTag("Player")) return;
            crafterView.CloseView();
        }
    }
}