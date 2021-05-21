using Runtime.Data;

namespace Runtime.Event {
    public sealed class PerformBuildEvent : EventData {
        public override EventType Type => EventType.PerformBuild;
        public BuildableObject BuildableObject { get; }
        
        public PerformBuildEvent(object sender, BuildableObject buildableObject) : base(sender) {
            BuildableObject = buildableObject;
        }
    }
}