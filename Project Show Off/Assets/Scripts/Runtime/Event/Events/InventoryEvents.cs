using Runtime.Data;

namespace Runtime.Event {
    public abstract class InventoryUpdateEvent : EventData {
        public override EventType Type => EventType.InventoryUpdate;
        public MaterialInventory Inventory { get; }

        protected InventoryUpdateEvent(object sender, MaterialInventory inventory) : base(sender) {
            Inventory = inventory;
        }
    }

    public sealed class MaterialInventoryUpdateEvent : InventoryUpdateEvent {
        public MaterialInventoryUpdateEvent(object sender, MaterialInventory inventory) : base(sender, inventory) { }
    }
    
    public sealed class PlaceableInventoryUpdateEvent : InventoryUpdateEvent {
        public PlaceableInventoryUpdateEvent(object sender, MaterialInventory inventory) : base(sender, inventory) { }
    }

    public sealed class InventoryResponseEvent : EventData {
        public override EventType Type => EventType.InventoryResponse;

        public MaterialInventory MaterialInventory { get; }
        public MaterialInventory PlaceableInventory { get; }
        
        public InventoryResponseEvent(object sender, MaterialInventory materialInventory, MaterialInventory placeableInventory) : base(sender) {
            MaterialInventory = materialInventory;
            PlaceableInventory = placeableInventory;
        }
    }
}