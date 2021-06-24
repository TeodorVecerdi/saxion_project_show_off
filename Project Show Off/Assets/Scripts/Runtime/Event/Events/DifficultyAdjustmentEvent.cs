namespace Runtime.Event {
    public sealed class DifficultyAdjustmentEvent : EventData {
        public override EventType Type => EventType.DifficultyAdjustment;
        
        public float Difficulty { get; }
        
        public DifficultyAdjustmentEvent(object sender, float difficulty) : base(sender) {
            Difficulty = difficulty;
        }
    }
}