using System;
using Runtime.Event;
using UnityEngine;

namespace Runtime {
    public class PlayerMovement : MonoBehaviour {

        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpHeight;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private Vector3 velocity;

        private void Awake() {
            controller = GetComponent<CharacterController>();
        }
        
        private void Update() {
            var grounded = controller.isGrounded;
            if (grounded && velocity.y < 0.0f) velocity.y = 0.0f;

            var delta = speed * Time.deltaTime * InputManager.PlayerMovement;
            var right = cameraTransform.right;
            var forward = Vector3.Cross(right, Vector3.up);
            var movement = right * delta.x + forward * delta.y;
            movement.y = 0.0f;
            controller.Move(movement);

            if (InputManager.WasJumpTriggered && grounded) 
                velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}