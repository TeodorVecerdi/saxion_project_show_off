namespace Runtime.Event {
    /// <summary>
    /// Serves as an empty event that can be of any type. Useful when you don't need extra data and just want to trigger an event of a specific type
    /// </summary>
    public sealed class EmptyEvent : EventData {
        public EmptyEvent(object sender, EventType eventType) : base(sender) {
            Type = eventType;
        }

        public override EventType Type { get; }
    }
}