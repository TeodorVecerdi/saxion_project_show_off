using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime {
    public class FirstPersonInputModule : StandaloneInputModule {
        private readonly MouseState mouseState = new MouseState();
        private Vector2 cursorPosition;
        
        // Hand this function your fake mouse position (in screen coords)
        public void UpdateCursorPosition(Vector2 position) {
            cursorPosition = position;
        }

        protected override MouseState GetMousePointerEventData() {
            Debug.Log("GetMousePointerEventData called");
            var created = GetPointerData(kMouseLeftId, out var leftData, true);

            leftData.Reset();

            if (created)
                leftData.position = cursorPosition;

            var pos = cursorPosition;
            leftData.delta = pos - leftData.position;
            leftData.position = pos;
            leftData.scrollDelta = Input.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            // copy the appropriate data into right and middle slots
            GetPointerData(kMouseRightId, out var rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            GetPointerData(kMouseMiddleId, out var middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;

            mouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
            mouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
            mouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

            return mouseState;
        }
    }
}