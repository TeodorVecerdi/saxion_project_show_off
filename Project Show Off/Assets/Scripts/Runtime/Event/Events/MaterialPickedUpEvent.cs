using Runtime.Data;

namespace Runtime.Event {
    public class MaterialPickedUpEvent : EventData {
        public override EventType Type => EventType.MaterialPickedUp;
        public ItemStack MaterialItemStack { get; }

        public MaterialPickedUpEvent(object sender, ItemStack materialItemStack) : base(sender) {
            MaterialItemStack = materialItemStack;
        }
    }
}