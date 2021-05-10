using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class LookWithMouse : MonoSingleton<LookWithMouse> {
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private Transform playerBody;

        private float xRotation;
        private bool isEnabled;

        public void SetEnabled(bool state) {
            isEnabled = state;
            if (isEnabled) {
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }

            Cursor.lockState = CursorLockMode.None;
        }

        private void Start() {
            SetEnabled(true);
        }

        private void Update() {
            if(!isEnabled) return;
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}