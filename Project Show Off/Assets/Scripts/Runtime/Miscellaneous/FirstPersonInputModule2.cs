using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime {
    public class FirstPersonInputModule2 : PointerInputModule {
        public string ClickInputName = "Submit";
        public RaycastResult CurrentRaycast;

        private PointerEventData pointerEventData;
        private GameObject currentLookAtHandler;

        public override void Process() {
            SetPointerPosition();
            HandleRaycast();
            HandleSelection();
        }

        private void SetPointerPosition() {
            if (pointerEventData == null) {
                pointerEventData = new PointerEventData(eventSystem);
            }

            // fake a pointer always being at the center of the screen
            pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            pointerEventData.delta = Vector2.zero;
        }

        private void HandleRaycast() {
            var raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, raycastResults);
            CurrentRaycast = pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);

            ProcessMove(pointerEventData);
            ProcessDrag(pointerEventData);
        }

        private void HandleSelection() {
            if (pointerEventData.pointerEnter != null) {
                GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);

                if (currentLookAtHandler != handler) {
                    currentLookAtHandler = handler;
                }

                if (currentLookAtHandler != null && Input.GetMouseButtonDown(0)) {
                    ExecuteEvents.ExecuteHierarchy(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
            } else {
                currentLookAtHandler = null;
            }
        }
    }
}