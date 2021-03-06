using Runtime.Data;

namespace Runtime.Event {
    public sealed class MaterialInventoryUpdateEvent : EventData {
        public override EventType Type => EventType.InventoryUpdate;
        public MaterialInventory Inventory { get; }

        public MaterialInventoryUpdateEvent(object sender, MaterialInventory inventory) : base(sender) {
            Inventory = inventory;
        }
    }

    public sealed class DepositInventoryUpdateEvent : EventData {
        public override EventType Type => EventType.DepositInventoryUpdate;
        public MaterialInventory Inventory { get; }

        public DepositInventoryUpdateEvent(object sender, MaterialInventory inventory) : base(sender) {
            Inventory = inventory;
        }
    }

    public sealed class DepositInventoryResponseEvent : EventData {
        public override EventType Type => EventType.DepositInventoryResponse;

        public MaterialInventory Inventory { get; }

        public DepositInventoryResponseEvent(object sender, MaterialInventory materialInventory) : base(sender) {
            Inventory = materialInventory;
        }
    }
}