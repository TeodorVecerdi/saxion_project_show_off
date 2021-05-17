using Runtime.Data;

namespace Runtime.Event {
    public sealed class CraftRequestEvent : EventData {
        public override EventType Type => EventType.CraftRequest;
        public CraftingRecipe Recipe { get; }
        
        public CraftRequestEvent(object sender, CraftingRecipe recipe) : base(sender) {
            Recipe = recipe;
        }
    }
}