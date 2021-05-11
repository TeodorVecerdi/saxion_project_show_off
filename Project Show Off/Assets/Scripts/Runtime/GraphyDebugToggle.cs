using UnityEngine;
using UnityEngine.InputSystem;

public class GraphyDebugToggle : MonoBehaviour {
    [SerializeField] private Canvas graphyCanvas;
    private bool isEnabled;

    private void SetEnabled(bool state) {
        isEnabled = state;
        graphyCanvas.enabled = isEnabled;
    }

    private void Start() {
        SetEnabled(true);
    }

    private void Update() {
        if (Keyboard.current.f1Key.wasPressedThisFrame) {
            SetEnabled(!isEnabled);
        }
    }
}

