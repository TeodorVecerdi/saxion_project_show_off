using Runtime.Data;

namespace Runtime.Event {
    public sealed class TrashEvent : EventData {
        public override EventType Type { get; }

        public Pickup Pickup { get; }

        public TrashEvent(object sender, EventType eventType, Pickup pickup) : base(sender) {
            Pickup = pickup;
            Type = eventType;
        }
    }

    public sealed class TrashPickupBinEvent : EventData {
        public override EventType Type => EventType.TrashPickupBin;
        
        public TrashPickup TrashPickup { get; }
        public float Mass { get; }
        
        public TrashPickupBinEvent(object sender, TrashPickup trashPickup, float mass) : base(sender) {
            TrashPickup = trashPickup;
            Mass = mass;
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