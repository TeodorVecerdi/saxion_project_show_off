using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class GeneralInput : MonoBehaviour {
        private bool isBuildModeActive;
        private void OnEnable() {
            InputManager.GeneralActions.ToggleGameMode.performed += ToggleGameMode;
        }

        private void OnDisable() {
            InputManager.GeneralActions.ToggleGameMode.performed -= ToggleGameMode;
        }

        private void ToggleGameMode(InputAction.CallbackContext callbackContext) {
            isBuildModeActive = !isBuildModeActive;
            EventQueue.QueueEvent(new ChangeMouseLockEvent(this, !isBuildModeActive));
            EventQueue.QueueEvent(new EmptyEvent(this, EventType.GameModeChange));
        }
    }
}