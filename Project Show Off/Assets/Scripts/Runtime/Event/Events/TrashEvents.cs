using Runtime.Data;

namespace Runtime.Event {
    public sealed class TrashPickupEvent : EventData {
        public override EventType Type { get; }

        public Pickup Pickup { get; }

        public TrashPickupEvent(object sender, EventType eventType, Pickup pickup) : base(sender) {
            Pickup = pickup;
            Type = eventType;
        }
    }

    public sealed class TrashPickupSpaceRequest : EventData {
        public override EventType Type => EventType.TrashPickupSpaceRequest;
        public float Mass { get; }

        public TrashPickupSpaceRequest(object sender, float mass) : base(sender) {
            Mass = mass;
        }
    }

    public sealed class TrashPickupSpaceResponse : EventData {
        public override EventType Type => EventType.TrashPickupSpaceResponse;
        public bool CanPickUp { get; }

        public TrashPickupSpaceResponse(object sender, bool canPickUp) : base(sender) {
            CanPickUp = canPickUp;
        }
    }
}