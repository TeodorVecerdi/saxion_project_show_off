﻿using Runtime.Data;

namespace Runtime.Event {
    public abstract class InventoryUpdateEvent : EventData {
        public override EventType Type => EventType.InventoryUpdate;
        public Inventory Inventory { get; }

        protected InventoryUpdateEvent(object sender, Inventory inventory) : base(sender) {
            Inventory = inventory;
        }
    }

    public sealed class MaterialInventoryUpdateEvent : InventoryUpdateEvent {
        public MaterialInventoryUpdateEvent(object sender, Inventory inventory) : base(sender, inventory) { }
    }
    
    public sealed class PlaceableInventoryUpdateEvent : InventoryUpdateEvent {
        public PlaceableInventoryUpdateEvent(object sender, Inventory inventory) : base(sender, inventory) { }
    }

    public sealed class InventoryResponseEvent : EventData {
        public override EventType Type => EventType.InventoryResponse;

        public Inventory MaterialInventory { get; }
        public Inventory PlaceableInventory { get; }
        
        public InventoryResponseEvent(object sender, Inventory materialInventory, Inventory placeableInventory) : base(sender) {
            MaterialInventory = materialInventory;
            PlaceableInventory = placeableInventory;
        }
    }
}