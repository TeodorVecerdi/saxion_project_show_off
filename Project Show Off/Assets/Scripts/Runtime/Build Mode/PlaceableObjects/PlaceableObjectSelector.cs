using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime {
    public class PlaceableObjectSelector : MonoBehaviour {
        [SerializeField] private Camera raycastCamera;
        private PlaceableSelectionHandler currentSelection;

        private void OnEnable() {
            InputManager.BuildModeActions.SelectObject.performed += OnSelectObjectPerformed;
        }

        private void OnDisable() {
            InputManager.BuildModeActions.SelectObject.performed -= OnSelectObjectPerformed;
        }

        private void OnSelectObjectPerformed(InputAction.CallbackContext callbackContext) {
            if (raycastCamera == null || Mouse.current == null) return;
            var mousePosition = Mouse.current.position.ReadValue();

            var ray = raycastCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000.0f, LayerMask.GetMask("Placeable"))) {
                var selection = hit.transform.GetComponent<PlaceableSelectionHandler>();
                if (selection == null || currentSelection == selection) return;
                if (currentSelection != null) 
                    currentSelection.SetSelection(false);
                currentSelection = selection;
                currentSelection.SetSelection(true);
            } else {
                if (currentSelection == null) return;
                currentSelection.SetSelection(false);
                currentSelection = null;
            }
        }
    }
}