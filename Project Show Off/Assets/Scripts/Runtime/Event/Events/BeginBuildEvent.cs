namespace Runtime.Event {
    public sealed class BeginBuildEvent : EventData {
        public override EventType Type => EventType.BeginBuild;
        public BuildableObjectPreview Prefab { get; }
        
        public BeginBuildEvent(object sender, BuildableObjectPreview prefab) : base(sender) {
            Prefab = prefab;
        }
    }
}