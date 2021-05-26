using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class MaterialDeposit : MonoBehaviour, IEventSubscriber {
        private const string kDepositHintFormat = "Press {0} to deposit inventory";

        [Header("Settings")]
        [SerializeField] private float depositUiFadeDuration = 0.25f;
        [SerializeField] private KeyCode depositKey = KeyCode.E;
        [Header("References")]
        [SerializeField] private CanvasGroup depositUICanvasGroup;
        [SerializeField] private MaterialInventory inventory;

        private List<IDisposable> eventUnsubscribeTokens;
        private bool canDeposit;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.DepositInventoryRequest), 
                this.Subscribe(EventType.PerformBuild)
            };
            depositUICanvasGroup.alpha = 0.0f;
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void OnEnable() {
            InputManager.PlayerActions.DepositInventory.performed += OnDepositPerformed;
        }
        
        private void OnDisable() {
            InputManager.PlayerActions.DepositInventory.performed -= OnDepositPerformed;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;
            canDeposit = true;
            depositUICanvasGroup.DOFade(1.0f, depositUiFadeDuration).From(0.0f);
            UIHintController.Instance.Add(string.Format(kDepositHintFormat, depositKey));
        }

        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Player")) return;
            canDeposit = false;
            depositUICanvasGroup.DOFade(0.0f, depositUiFadeDuration).From(1.0f);
            UIHintController.Instance.Remove(string.Format(kDepositHintFormat, depositKey));
        }

        private void OnDepositPerformed(InputAction.CallbackContext obj) {
            if(!canDeposit) return;
            EventQueue.QueueEvent(new DepositMaterialsRequestEvent(this, inventory));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.DepositInventoryRequest}: {
                    EventQueue.QueueEvent(new DepositInventoryResponseEvent(this, inventory));
                    return false;
                }
                case PerformBuildEvent performBuildEvent: {
                    inventory.Remove(performBuildEvent.BuildableObject.ConstructionRequirements);
                    EventQueue.QueueEvent(new DepositInventoryUpdateEvent(this, inventory));
                    return false;
                }
                default: return false;
            }
        }
    }
}