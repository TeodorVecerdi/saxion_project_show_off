using System;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class ItemPickup : MonoBehaviour, IEventSubscriber {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform cameraTransform;
        
        private Pickup pickupUnderMouse;
        private bool shouldRaycast;
        private IDisposable itemPickupSuccessEventUnsubscriber;

        private void Awake() {
            itemPickupSuccessEventUnsubscriber = EventQueue.Subscribe(this, EventType.ItemPickupSuccess);
        }

        private void OnDestroy() {
            itemPickupSuccessEventUnsubscriber.Dispose();
        }

        private void OnEnable() {
            InputManager.PlayerActions.PickUp.performed += PickupPerformed;
        }

        private void OnDisable() {
            InputManager.PlayerActions.PickUp.performed -= PickupPerformed;
        }

        private void Update() {
            if (shouldRaycast) {
                shouldRaycast = false;
                DoPickupRaycast();
                return;
            }
            if (transform.hasChanged) {
                transform.hasChanged = false;
                DoPickupRaycast();
            } else if (cameraTransform.hasChanged) {
                cameraTransform.hasChanged = false;
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
            EventQueue.QueueEvent(new ItemPickupEvent(this, EventType.ItemPickupRequest, pickupUnderMouse));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case ItemPickupEvent {Type: EventType.ItemPickupSuccess} itemPickupSuccessEvent: {
                    Destroy(itemPickupSuccessEvent.Pickup.gameObject);
                    pickupUnderMouse = null;
                    shouldRaycast = true;
                    return true;
                }
                default: return false;
            }
        }
    }
}

