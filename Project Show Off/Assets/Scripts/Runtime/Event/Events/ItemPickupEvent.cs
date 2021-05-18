namespace Runtime.Event {
    public sealed class ItemPickupEvent : EventData {
        public override EventType Type { get; }
        
        public Pickup Pickup { get; }
        
        public ItemPickupEvent(object sender, EventType eventType, Pickup pickup) : base(sender) {
            Pickup = pickup;
            Type = eventType;
        }
    }
}