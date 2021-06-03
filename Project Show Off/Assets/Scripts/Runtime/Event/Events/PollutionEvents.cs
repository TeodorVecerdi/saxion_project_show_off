namespace Runtime.Event {
    public sealed class PollutionUpdateEvent : EventData {
        public override EventType Type => EventType.PollutionUpdate;
        public float RawPollution { get; }
        public float PollutionRatio { get; }
        
        public PollutionUpdateEvent(object sender, float rawPollution, float pollutionRatio) : base(sender) {
            RawPollution = rawPollution;
            PollutionRatio = pollutionRatio;
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