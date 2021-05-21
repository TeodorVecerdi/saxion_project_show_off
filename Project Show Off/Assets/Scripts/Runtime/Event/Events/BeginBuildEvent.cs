using Runtime.Data;

namespace Runtime.Event {
    public sealed class BeginBuildEvent : EventData {
        public override EventType Type => EventType.BeginBuild;
        public BuildableObject BuildableObject { get; }
        
        public BeginBuildEvent(object sender, BuildableObject  buildableObject) : base(sender) {
            BuildableObject = buildableObject;
        }
    }
}