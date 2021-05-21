namespace Runtime.Event {
    public sealed class ItemPickupEvent : EventData {
        public override EventType Type { get; }
        
        public Pickup Pickup { get; }
        
        public ItemPickupEvent(object sender, EventType eventType, Pickup pickup) : base(sender) {
            Pickup = pickup;
            Type = eventType;
        }
    }
    
    public sealed class ItemPickupSpaceRequest : EventData {
        public override EventType Type => EventType.ItemPickupSpaceRequest;
        public float Mass { get; }
        
        public ItemPickupSpaceRequest(object sender, float mass) : base(sender) {
            Mass = mass;
        }
    }
    
    public sealed class ItemPickupSpaceResponse : EventData {
        public override EventType Type => EventType.ItemPickupSpaceResponse;
        public bool CanPickUp { get; }
        
        public ItemPickupSpaceResponse(object sender, bool canPickUp) : base(sender) {
            CanPickUp = canPickUp;
        }
    }
}