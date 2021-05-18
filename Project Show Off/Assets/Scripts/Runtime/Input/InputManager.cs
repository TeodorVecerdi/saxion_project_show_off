using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime {
    internal static class InputManager {
        private static DefaultInputActions actions;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad() {
            actions = new DefaultInputActions();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeAfterSceneLoad() {
            Enable();
        }

        internal static DefaultInputActions.PlayerActions PlayerActions => actions.Player;
        internal static DefaultInputActions.UIActions UIActions => actions.UI;
        internal static DefaultInputActions.BuildModeActions BuildModeActions => actions.BuildMode;
        internal static DefaultInputActions.GeneralActions GeneralActions => actions.General;
        
        // PlayerActions
        internal static Vector2 PlayerMovement => PlayerActions.Move.ReadValue<Vector2>(); 
        internal static bool WasJumpTriggered => PlayerActions.Jump.triggered;
        internal static bool WasPickupTriggered => PlayerActions.PickUp.triggered;
        internal static bool WasOpenMenuTriggered => PlayerActions.OpenMenu.triggered;
        internal static Vector2 MouseDelta => PlayerActions.Look.ReadValue<Vector2>();
        
        // BuildModeActions
        internal static Vector2 CameraKeyboardPan => BuildModeActions.KeyboardPan.ReadValue<Vector2>();
        internal static float Rotation => BuildModeActions.Rotation.ReadValue<float>();
        internal static float RawZoom => BuildModeActions.Zoom.ReadValue<float>();
        internal static float KeyboardZoom => BuildModeActions.KeyboardZoom.ReadValue<float>();
        internal static bool IsBoosting => BuildModeActions.Boost.phase == InputActionPhase.Started;
        
        // GeneralActions
        internal static bool WasGameModeSwitchTriggered => GeneralActions.ToggleGameMode.triggered;
        
        internal static void Enable() {
            actions.Enable();
        }

        internal static void Disable() {
            actions.Disable();
        }
    }
}