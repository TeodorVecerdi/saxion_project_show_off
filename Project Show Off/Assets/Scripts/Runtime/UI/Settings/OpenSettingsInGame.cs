using DG.Tweening;
using Runtime.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime {
    public sealed class OpenSettingsInGame : MonoBehaviour {
        [SerializeField] private SettingsController settingsController;

        private CanvasGroup settingsCanvasGroup;
        private bool isSettingsMenuOpen;

        private void Awake() {
            settingsCanvasGroup = settingsController.GetComponent<CanvasGroup>();
        }

        private void OnEnable() {
            InputManager.GeneralActions.ToggleSettings.performed += ToggleSettings;
        }

        private void OnDisable() {
            InputManager.GeneralActions.ToggleSettings.performed -= ToggleSettings;
        }

        private void ToggleSettings(InputAction.CallbackContext obj) {
            isSettingsMenuOpen = !isSettingsMenuOpen;
            settingsController.SetEnabled(isSettingsMenuOpen);

            if (isSettingsMenuOpen) settingsController.gameObject.SetActive(true);
            settingsCanvasGroup.DOFade(isSettingsMenuOpen ? 1.0f : 0.0f, 0.25f).SetUpdate(true).OnComplete(() => {
                if (!isSettingsMenuOpen) settingsCanvasGroup.gameObject.SetActive(false);
            });

            if (isSettingsMenuOpen) InputManager.GeneralActions.ToggleGameMode.Disable();
            else InputManager.GeneralActions.ToggleGameMode.Enable();

            if (GeneralInput.IsBuildModeActive) {
                if (isSettingsMenuOpen) InputManager.BuildModeActions.Disable();
                else InputManager.BuildModeActions.Enable();
            } else {
                if (isSettingsMenuOpen) InputManager.PlayerActions.Disable();
                else InputManager.PlayerActions.Enable();

                // Unlock / Lock the mouse in case it's in play mode
                EventQueue.QueueEvent(new ChangeMouseLockEvent(this, !isSettingsMenuOpen));
            }
        }
    }
}