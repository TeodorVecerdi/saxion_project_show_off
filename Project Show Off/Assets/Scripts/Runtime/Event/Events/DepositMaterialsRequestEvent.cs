using Runtime.Data;

namespace Runtime.Event {
    public sealed class DepositMaterialsRequestEvent : EventData {
        public override EventType Type => EventType.DepositMaterialsRequest;
        public MaterialInventory DepositInventory { get; }
        
        public DepositMaterialsRequestEvent(object sender, MaterialInventory depositInventory) : base(sender) {
            DepositInventory = depositInventory;
        }
    }
}