using Runtime.Data;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class Pickup : MonoBehaviour {
        public TrashPickup TrashPickup { get; private set; }
        public float Mass { get; private set; }

        private new Rigidbody rigidbody;

        private void Awake() {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void Load(TrashPickup trashPickup) {
            TrashPickup = trashPickup;
            Mass = Rand.Range(TrashPickup.MinimumMass, TrashPickup.MaximumMass).RoundedTo(0.1f).Clamped(TrashPickup.MinimumMass, TrashPickup.MaximumMass);
        }

        public void StartPickup() {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        public void StopPickup() {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
    }
}