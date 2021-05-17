namespace Runtime.Event {
    public sealed class ChangeMouseLockEvent : EventData {
        public override EventType Type => EventType.ChangeMouseLock;
        public bool State { get; }
        
        public ChangeMouseLockEvent(object sender, bool state) : base(sender) {
            State = state;
        }
    }
}