using System;
using Runtime.Data;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class Pickup : MonoBehaviour {
        [SerializeField] private Item item;
        [SerializeField] private float mass;

        public Item Item => item;
        public float Mass => mass;

        private new Rigidbody rigidbody;

        private void Awake() {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            mass = Rand.Range(item.MinimumMass, item.MaximumMass).RoundedTo(0.1f).Clamped(item.MinimumMass, item.MaximumMass);
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