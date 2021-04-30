namespace Runtime.Event {
    public abstract class EventData {
        public abstract EventType Type { get; }
        public object Sender { get; }

        protected EventData(object sender) {
            Sender = sender;
        }
    }
}