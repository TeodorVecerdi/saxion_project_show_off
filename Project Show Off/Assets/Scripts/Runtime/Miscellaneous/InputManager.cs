using UnityEngine;
using UnityEngine.InputSystem;

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

        public static DefaultInputActions.PlayerActions PlayerActions => actions.Player;
        public static DefaultInputActions.UIActions UIActions => actions.UI;
        public static DefaultInputActions.BuildModeActions BuildModeActions => actions.BuildMode;
        
        // PlayerActions
        public static Vector2 PlayerMovement => PlayerActions.Move.ReadValue<Vector2>(); 
        public static bool WasJumpTriggered => PlayerActions.Jump.triggered;
        public static bool WasPickupTriggered => PlayerActions.PickUp.triggered;
        public static bool WasOpenMenuTriggered => PlayerActions.OpenMenu.triggered;
        public static Vector2 MouseDelta => PlayerActions.Look.ReadValue<Vector2>();
        
        // BuildModeActions
        public static Vector2 CameraKeyboardPan => BuildModeActions.KeyboardPan.ReadValue<Vector2>();
        public static Vector2 Zoom => BuildModeActions.Zoom.ReadValue<Vector2>();
        public static bool IsBoosting => BuildModeActions.Boost.phase == InputActionPhase.Started;
        
        
        public static void Enable() {
            actions.Enable();
        }

        public static void Disable() {
            actions.Disable();
        }
    }
}