using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime {
    public class LookWithMouse : MonoBehaviour {
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private Transform playerBody;

        private float xRotation;

        internal static void SetMouseLock(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Start() {
            SetMouseLock(true);
        }

        private void Update() {
            if(InputManager.PlayerActions.Look.phase != InputActionPhase.Started) return;
        
            var delta = InputManager.MouseDelta * (mouseSensitivity * Time.deltaTime * 0.06666667f);
            xRotation -= delta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * delta.x);
        }
    }
}