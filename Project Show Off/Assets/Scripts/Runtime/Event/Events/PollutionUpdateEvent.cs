namespace Runtime.Event {
    public sealed class PollutionUpdateEvent : EventData {
        public override EventType Type => EventType.PollutionUpdate;
        public float Pollution { get; }
        
        public PollutionUpdateEvent(object sender, float pollution) : base(sender) {
            Pollution = pollution;
        }
    }
}