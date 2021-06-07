using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Tutorial {
    public class MovementTutorial : TutorialSlide {
        public override string TutorialKey => "movement";
        
        [HorizontalLine(color: EColor.Orange, order = -10000), Header("Movement Tutorial", order = -20000)]
        [SerializeField] private float movementTimeRequired = 3.0f;

        private float totalTimeMoved;
        private bool isMoving;

        private void OnEnable() {
            InputManager.PlayerActions.Move.started += OnPlayerMoveStarted;
            InputManager.PlayerActions.Move.canceled += OnPlayerMoveCanceled;
        }

        private void OnDisable() {
            InputManager.PlayerActions.Move.started -= OnPlayerMoveStarted;
            InputManager.PlayerActions.Move.canceled -= OnPlayerMoveCanceled;
        }

        protected override void Process() {
            if (!isMoving) return;
            
            totalTimeMoved += Time.deltaTime;
            FillAmount = totalTimeMoved / movementTimeRequired;
            if (totalTimeMoved >= movementTimeRequired)
                FinishTutorial();
        }

        private void OnPlayerMoveStarted(InputAction.CallbackContext obj) {
            isMoving = true;
        }

        private void OnPlayerMoveCanceled(InputAction.CallbackContext obj) {
            isMoving = false;
        }
    }
}