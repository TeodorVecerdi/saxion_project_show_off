using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PlayerInventory : MonoBehaviour, IEventSubscriber {
        [SerializeField] private EventQueue eventQueue;
        [SerializeField] private Inventory materialInventory;
        [SerializeField] private Inventory placeableInventory;

        private readonly List<IDisposable> eventUnsubscribeTokens = new List<IDisposable>();

        private void Awake() {
            eventUnsubscribeTokens.Add(eventQueue.Subscribe(this, EventType.MaterialPickedUp));
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Crafter")) {
                // Todo: open crafter UI, load recipes, etc
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Crafter")) {
                // Todo: close crafter UI
            }
        }

        private void OnDestroy() {
            eventUnsubscribeTokens.ForEach(token => token.Dispose());
            eventUnsubscribeTokens.Clear();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            if (eventData is MaterialPickedUpEvent materialPickedUpEvent) {
                materialInventory.Add(materialPickedUpEvent.MaterialItemStack);
                // eventQueue.QueueEvent(new InventoryUpdateEvent(this, materialInventory, placeableInventory));
                return false;
            }

            return false;
        }
    }
}