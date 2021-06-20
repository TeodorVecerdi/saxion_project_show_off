using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class PlayerInventory : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float maximumCarryMass = 50.0f;
        [SerializeField] private MaterialInventory materialInventory;

        public float MaximumCarryMass => maximumCarryMass;
        public MaterialInventory MaterialInventory => materialInventory;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupRequest), 
                this.Subscribe(EventType.DepositMaterialsRequest), 
                this.Subscribe(EventType.TrashPickupSpaceRequest)
            };
            materialInventory ??= new MaterialInventory();
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
                case TrashEvent {Type: EventType.TrashPickupRequest} itemPickupEvent: {
                    var mass = itemPickupEvent.Pickup.Mass;
                    if (materialInventory.TotalMass + mass > MaximumCarryMass) return true;
                    materialInventory.Add(itemPickupEvent.Pickup.TrashPickup.TrashMaterial, itemPickupEvent.Pickup.Mass);
                    EventQueue.QueueEvent(new TrashEvent(this, EventType.TrashPickupSuccess, itemPickupEvent.Pickup));
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    return true;
                }
                case DepositMaterialsRequestEvent depositMaterialsRequestEvent: {
                    depositMaterialsRequestEvent.DepositInventory.Add(materialInventory);
                    materialInventory.Clear();
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    EventQueue.QueueEvent(new DepositInventoryUpdateEvent(this, depositMaterialsRequestEvent.DepositInventory));
                    return false;
                }
                case TrashPickupSpaceRequest itemPickupSpaceRequest: {
                    EventQueue.QueueEvent(new TrashPickupSpaceResponse(this, materialInventory.TotalMass + itemPickupSpaceRequest.Mass <= MaximumCarryMass));
                    return true;
                }
                default: return false;
            }
        }
    }
}