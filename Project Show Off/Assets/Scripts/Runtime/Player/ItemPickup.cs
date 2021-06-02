using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Runtime.Event;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class ItemPickup : MonoBehaviour, IEventSubscriber {
        [SerializeField] /*debug:*/ private TextMeshProUGUI text;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Image pickupIndicatorImage;
        [SerializeField] private Transform vacuumEndTransform;
        [Space]
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private AnimationCurve scaleCurve;
        [Tooltip("The time it takes for the object scale to return to normal after cancelling a pickup")]
        [SerializeField] private float unscaleTransitionDuration = 0.1f;

        private Pickup pickupUnderMouse;
        private Vector3 initialPickupLocation;
        private float pickupTimer;
        private bool shouldRaycast;
        private bool isPickingUp;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupSuccess),
                this.Subscribe(EventType.TrashPickupSpaceResponse)
            };

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
                var fillAmount = pickupTimer / pickupUnderMouse.TrashPickup.PickupDuration;
                var pickupPosition = Vector3.Lerp(initialPickupLocation, vacuumEndTransform.position, movementCurve.Evaluate(fillAmount));
                var pickupScale = 1.0f-Mathf.Lerp(0.0f, 1.0f, scaleCurve.Evaluate(fillAmount));
                
                //!! Reason: repeated property access of built in component is inefficient
                var pickupTransform = pickupUnderMouse.transform; 
                pickupTransform.position = pickupPosition;
                pickupTransform.localScale = Vector3.one * pickupScale;
                
                pickupIndicatorImage.fillAmount = fillAmount;
                
                if (fillAmount >= 1.0f) {
                    EventQueue.QueueEvent(new TrashPickupEvent(this, EventType.TrashPickupRequest, pickupUnderMouse));
                    StopPickup();
                }
            } else {
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
        }

        private void DoPickupRaycast() {
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out var hitInfo, 10, LayerMask.GetMask("Pickup"))) {
                var pickup = hitInfo.transform.GetComponent<Pickup>();
                if (pickup != pickupUnderMouse && isPickingUp)
                    StopPickup();

                pickupUnderMouse = pickup;
                text.text = $"{pickupUnderMouse.TrashPickup.ItemName} ({pickupUnderMouse.TrashPickup.TrashCategory.CategoryName})";
            } else {
                if (pickupUnderMouse != null && isPickingUp)
                    StopPickup();

                pickupUnderMouse = null;
                text.text = "None";
            }
        }

        private void PickupCanceled(InputAction.CallbackContext context) {
            if(pickupUnderMouse == null) return;
            
            StopPickup();
            pickupUnderMouse.transform.DOScale(Vector3.one, unscaleTransitionDuration);
            pickupUnderMouse.StopPickup();
        }

        private void PickupStarted(InputAction.CallbackContext context) {
            if (pickupUnderMouse == null) return;
            EventQueue.QueueEvent(new TrashPickupSpaceRequest(this, pickupUnderMouse.Mass));
        }

        private void StartPickup() {
            isPickingUp = true;
            pickupIndicatorImage.fillAmount = 0.0f;
            pickupTimer = 0.0f;
            initialPickupLocation = pickupUnderMouse.transform.position;
            pickupUnderMouse.StartPickup();
            SoundManager.PlaySound("Vacuum");
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
                    if (itemPickupSpaceResponse.CanPickUp) StartPickup();
                    return true;
                }
                default: return false;
            }
        }
    }
}