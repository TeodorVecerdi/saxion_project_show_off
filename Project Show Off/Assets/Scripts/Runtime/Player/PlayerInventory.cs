using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PlayerInventory : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float maximumCarryMass = 50.0f;
        [SerializeField] private MaterialInventory materialInventory;
        [SerializeField] private MaterialInventory placeableInventory;

        public float MaximumCarryMass => maximumCarryMass;
        public MaterialInventory MaterialInventory => materialInventory;
        public MaterialInventory PlaceableInventory => placeableInventory;

        private readonly List<IDisposable> eventUnsubscribeTokens = new List<IDisposable>();

        private void Awake() {
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.ItemPickupRequest));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.CraftRequest));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.DepositMaterialsRequest));
            eventUnsubscribeTokens.Add(EventQueue.Subscribe(this, EventType.ItemPickupSpaceRequest));
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
                case ItemPickupEvent {Type: EventType.ItemPickupRequest} itemPickupEvent: {
                    var mass = itemPickupEvent.Pickup.Mass;
                    if(materialInventory.TotalMass + mass > MaximumCarryMass) return true;
                    materialInventory.Add(itemPickupEvent.Pickup.Item.TrashCategory, itemPickupEvent.Pickup.Mass);
                    EventQueue.QueueEvent(new ItemPickupEvent(this, EventType.ItemPickupSuccess, itemPickupEvent.Pickup));
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    return true;
                }
                case CraftRequestEvent craftRequestEvent: {
                    if (!materialInventory.Contains(craftRequestEvent.Recipe.Ingredients)) return true;
                    
                    materialInventory.Remove(craftRequestEvent.Recipe.Ingredients);
                    materialInventory.Add(craftRequestEvent.Recipe.Result);
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    
                    // TODO!: Change to placeable inventory once system is in place 
                    // placeableInventory.Add(craftRequestEvent.Recipe.Result);
                    // EventQueue.QueueEvent(new PlaceableInventoryUpdateEvent(this, placeableInventory));
                    return true;
                }
                case DepositMaterialsRequestEvent depositMaterialsRequestEvent: {
                    depositMaterialsRequestEvent.DepositInventory.Add(materialInventory);
                    materialInventory.Clear();
                    EventQueue.QueueEvent(new MaterialInventoryUpdateEvent(this, materialInventory));
                    EventQueue.QueueEvent(new DepositInventoryUpdateEvent(this, depositMaterialsRequestEvent.DepositInventory));
                    return false;
                }
                case ItemPickupSpaceRequest itemPickupSpaceRequest: {
                    EventQueue.QueueEvent(new ItemPickupSpaceResponse(this, materialInventory.TotalMass + itemPickupSpaceRequest.Mass <= MaximumCarryMass));
                    return true;
                }
                default: return false;
            }
        }
    }
}