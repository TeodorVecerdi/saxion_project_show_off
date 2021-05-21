namespace Runtime.Event {
    public enum EventType {
        ItemPickupSuccess,
        ItemPickupRequest,
        ItemPickupSpaceRequest,
        ItemPickupSpaceResponse,
        InventoryUpdate,
        GameModeToggle, // triggered when components should respond to the change
        GameModeChange, // triggered when starting a game mode transition
        DepositMaterialsRequest,
        ChangeMouseLock,
        DepositInventoryRequest,
        DepositInventoryResponse,
        DepositInventoryUpdate,
        BeginBuild,
        CancelBuild,
        PerformBuild,
    }
}