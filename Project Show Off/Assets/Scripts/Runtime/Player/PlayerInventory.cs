using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PlayerInventory : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float maximumCarryMass = 50.0f;
        [SerializeField] private Inventory materialInventory;
        [SerializeField] private Inventory placeableInventory;

        public float MaximumCarryMass => maximumCarryMass;
        public Inventory MaterialInventory => materialInventory;
        public Inventory PlaceableInventory => placeableInventory;

        private readonly List<IDisposable> eventUnsubscribeTokens = new List<IDisposable>();

        private void Awake() {
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.MaterialPickedUp));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.InventoryRequest));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.CraftRequest));
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
            switch (eventData) {
                case MaterialPickedUpEvent materialPickedUpEvent: {
                    materialInventory.Add(materialPickedUpEvent.MaterialItemStack);
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    return false;
                }
                case EmptyEvent {Type: EventType.InventoryRequest}: {
                    EventQueue.QueueEvent(new InventoryResponseEvent(this, materialInventory, placeableInventory));
                    return true;
                }
                case CraftRequestEvent craftRequestEvent: {
                    if (!materialInventory.Contains(craftRequestEvent.Recipe.Ingredients)) return true;
                    
                    materialInventory.Remove(craftRequestEvent.Recipe.Ingredients);
                    // TODO!: Change to placeable inventory once system is in place 
                    materialInventory.Add(craftRequestEvent.Recipe.Result);
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    
                    // placeableInventory.Add(craftRequestEvent.Recipe.Result);
                    // EventQueue.QueueEvent(new PlaceableInventoryUpdateEvent(this, placeableInventory));
                    return true;
                }
                default: return false;
            }
        }
    }
}