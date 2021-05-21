using System;
using System.Collections.Generic;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class ItemPickup : MonoBehaviour, IEventSubscriber {
        /*debug:*/[SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Image pickupIndicatorImage;
        
        private Pickup pickupUnderMouse;
        private float pickupTimer;
        private bool shouldRaycast;
        private bool isPickingUp;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable>();
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.TrashPickupSuccess));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.TrashPickupSpaceResponse));
            
            pickupIndicatorImage.fillAmount = 0.0f;
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void OnEnable() {
            InputManager.PlayerActions.PickUp.started += PickupStarted;
            InputManager.PlayerActions.PickUp.canceled += PickupCanceled;
        }

        private void OnDisable() {
            InputManager.PlayerActions.PickUp.started -= PickupStarted;
            InputManager.PlayerActions.PickUp.canceled -= PickupCanceled;
        }

        private void Update() {
            if (isPickingUp) {
                pickupTimer += Time.deltaTime;
                var fillAmount = pickupTimer / pickupUnderMouse.Item.PickupDuration;
                pickupIndicatorImage.fillAmount = fillAmount;
                if (fillAmount >= 1.0f) {
                    EventQueue.QueueEvent(new TrashPickupEvent(this, EventType.TrashPickupRequest, pickupUnderMouse));
                    StopPickup();
                }
            }
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
                var pickup = hitInfo.transform.GetComponent<Pickup>();
                if(pickup != pickupUnderMouse && isPickingUp) 
                    StopPickup();
                
                pickupUnderMouse = pickup;
                text.text = $"{pickupUnderMouse.Item.ItemName} ({pickupUnderMouse.Item.TrashCategory.CategoryName})";
            } else {
                if(pickupUnderMouse != null && isPickingUp) 
                    StopPickup();
                
                pickupUnderMouse = null;
                text.text = "None";
            }
        }

        private void PickupCanceled(InputAction.CallbackContext context) {
            StopPickup();
        }

        private void PickupStarted(InputAction.CallbackContext context) {
            if (pickupUnderMouse == null) return;
            EventQueue.QueueEvent(new TrashPickupSpaceRequest(this, pickupUnderMouse.Mass));
        }

        private void StartPickup() {
            isPickingUp = true;
            pickupIndicatorImage.fillAmount = 0.0f;
            pickupTimer = 0.0f;
        }

        private void StopPickup() {
            isPickingUp = false;
            pickupIndicatorImage.fillAmount = 0.0f;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case TrashPickupEvent {Type: EventType.TrashPickupSuccess} itemPickupSuccessEvent: {
                    Destroy(itemPickupSuccessEvent.Pickup.gameObject);
                    pickupUnderMouse = null;
                    shouldRaycast = true;
                    return true;
                }
                case TrashPickupSpaceResponse itemPickupSpaceResponse: {
                    if(itemPickupSpaceResponse.CanPickUp)
                        StartPickup();
                    return true;
                }
                default: return false;
            }
        }
    }
}

