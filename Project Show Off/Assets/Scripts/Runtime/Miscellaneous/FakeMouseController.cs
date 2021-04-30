using UnityEngine;

namespace Runtime {
    public class FakeMouseController : MonoBehaviour {
        public static bool LockMouse;

        [SerializeField] private FirstPersonInputModule inputModule; 

        private Vector3 lastMousePosition;
        private Vector3 screenMiddle;

        private void Start() {
            lastMousePosition = Input.mousePosition;
            screenMiddle = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);
        }

        private void Update() {
            var mousePosition = Input.mousePosition;
            if (!LockMouse) {
                inputModule.UpdateCursorPosition(mousePosition);
                return;
            }

            var delta = mousePosition - lastMousePosition;
            lastMousePosition = mousePosition;
            inputModule.UpdateCursorPosition(screenMiddle + delta);
            Debug.Log($"Mouse position: {screenMiddle + delta}");
        }
    }
}