using UnityEngine;

namespace Runtime {
    public static class InputManager {
        private static DefaultInputActions actions;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad() {
            actions = new DefaultInputActions();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeAfterSceneLoad() {
            Enable();
        }

        public static DefaultInputActions.PlayerActions Player => actions.Player;
        public static DefaultInputActions.UIActions UI => actions.UI;
        
        public static Vector2 GetPlayerMovement() => Player.Move.ReadValue<Vector2>();
        public static Vector2 GetPlayerLook() => Player.Look.ReadValue<Vector2>();
        public static bool WasJumpTriggered() => Player.Jump.triggered;
        public static bool WasPickupTriggered() => Player.PickUp.triggered;
        public static bool WasOpenMenuTriggered() => Player.OpenMenu.triggered;
        
        public static void Enable() {
            actions.Enable();
        }

        public static void Disable() {
            actions.Disable();
        }
    }
}