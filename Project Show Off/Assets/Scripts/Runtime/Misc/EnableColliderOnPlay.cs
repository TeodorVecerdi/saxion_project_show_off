using UnityEngine;

namespace Runtime {
    public sealed class EnableColliderOnPlay : MonoBehaviour {
        [SerializeField] private Collider collider;

        private void Awake() {
            collider.enabled = true;
        }
    }
}