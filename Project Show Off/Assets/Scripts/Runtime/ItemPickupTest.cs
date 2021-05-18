using System;
using Runtime.Data;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime {
    public class ItemPickupTest : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform cameraTransform;
        private Pickup pickupUnderMouse;

        private void OnEnable() {
            InputManager.PlayerActions.PickUp.performed += PickupPerformed;
        }

        private void OnDisable() {
            InputManager.PlayerActions.PickUp.performed -= PickupPerformed;
        }


        private void Update() {
            if (transform.hasChanged) {
                transform.hasChanged = false;
                DoPickupRaycast();
            }
        }

        private void DoPickupRaycast() {
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out var hitInfo, 10, LayerMask.GetMask("Pickup"))) {
                pickupUnderMouse = hitInfo.transform.GetComponent<Pickup>();
                text.text = $"{pickupUnderMouse.Item.ItemName} ({pickupUnderMouse.Item.TrashCategory.CategoryName})";
            } else {
                pickupUnderMouse = null;
                text.text = "None";
            }
        }

        private void PickupPerformed(InputAction.CallbackContext context) {
            if (pickupUnderMouse == null) return;
            
            Destroy(pickupUnderMouse.gameObject);
            EventQueue.QueueEvent(new MaterialPickedUpEvent(this, new ItemStack(pickupUnderMouse.Item.TrashCategory, pickupUnderMouse.Mass)));
            pickupUnderMouse = null;
            DoPickupRaycast();
        }
    }
}

