namespace Runtime.Event {
    public sealed class PollutionUpdateEvent : EventData {
        public override EventType Type => EventType.PollutionUpdate;
        public float Pollution { get; }
        
        public PollutionUpdateEvent(object sender, float pollution) : base(sender) {
            Pollution = pollution;
        }
    }
    
    public sealed class PollutionChangeEvent : EventData {
        public override EventType Type => EventType.PollutionChange;
        public float Delta { get; }
        
        public PollutionChangeEvent(object sender, float delta) : base(sender) {
            Delta = delta;
        }
    }

}