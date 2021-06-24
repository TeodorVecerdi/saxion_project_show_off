namespace Runtime.Event {
    public enum EventType {
        InventoryUpdate,
        GameModeToggle, // triggered when components should respond to the change
        GameModeChange, // triggered when starting a game mode transition
        ChangeMouseLock,
        
        DepositMaterialsRequest,
        DepositInventoryRequest,
        DepositInventoryResponse,
        DepositInventoryUpdate,
        
        BeginBuild,
        CancelBuild,
        PerformBuild,
        
        TrashSpawn,
        TrashPickupSuccess,
        TrashPickupRequest,
        TrashPickupSpaceRequest,
        TrashPickupSpaceResponse,
        TrashPickupBin,
        NpcThrowTrash,
        
        PollutionUpdate,
        PollutionChange,
        
        SettingsChanged,
        ResetTutorial,
        
        BarUpdate,
        
        DifficultyAdjustment
    }
}