namespace Runtime.Event {
    /// <summary>
    /// Serves as an empty event that can be of any type. Useful when you don't need extra data and just want to trigger an event of a specific type
    /// </summary>
    public sealed class ChangeMouseLockEvent : EventData {
        public override EventType Type => EventType.ChangeMouseLock;
        public bool State { get; }
        
        public ChangeMouseLockEvent(object sender, bool state) : base(sender) {
            State = state;
        }
    }
}