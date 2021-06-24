namespace Runtime.Event {
    public sealed class BarUpdateEvent : EventData {
        public override EventType Type => EventType.BarUpdate;
        
        public float PeopleHappiness { get; }
        public float BiodiversityHappiness { get; }
        
        public BarUpdateEvent(object sender, float peopleHappiness, float biodiversityHappiness) : base(sender) {
            PeopleHappiness = peopleHappiness;
            BiodiversityHappiness = biodiversityHappiness;
        }
    }
}