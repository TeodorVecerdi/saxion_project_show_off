using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Tutorial {
    public class MovementTutorial : TutorialSlide {
        [Space]
        [SerializeField] private float movementTimeRequired = 3.0f;
        
        protected override string TutorialKey => "movement";

        private void OnEnable() {
            InputManager.PlayerActions.Move.started += OnPlayerMoveStarted;
            InputManager.PlayerActions.Move.canceled += OnPlayerMoveCanceled;
        }

        private void OnDisable() {
            InputManager.PlayerActions.Move.started -= OnPlayerMoveStarted;
            InputManager.PlayerActions.Move.canceled -= OnPlayerMoveCanceled;
        }

        protected override void Process() {
            Debug.Log("Process tutorial");
        }

        private void OnPlayerMoveStarted(InputAction.CallbackContext obj) {
            Debug.Log("Movement started");
        }

        private void OnPlayerMoveCanceled(InputAction.CallbackContext obj) {
            Debug.Log("Movement canceled");
        }
    }
}