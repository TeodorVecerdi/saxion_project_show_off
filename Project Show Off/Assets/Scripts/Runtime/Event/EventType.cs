namespace Runtime.Event {
    public enum EventType {
        ItemPickupSuccess,
        ItemPickupRequest,
        InventoryUpdate,
        GameModeToggle, // triggered when components should respond to the change
        GameModeChange, // triggered when starting a game mode transition
        ChangeMouseLock,
        InventoryRequest,
        InventoryResponse,
        CraftRequest
    }
}