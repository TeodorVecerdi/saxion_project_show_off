using System;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class PlayerMovement : MonoSingleton<PlayerMovement> {
        [SerializeField] private CharacterController controller;

        [SerializeField] private float speed = 12f;
        [SerializeField] private float gravity = -10f;
        [SerializeField] private float jumpHeight = 2f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;

        private bool isGrounded;
        private bool isEnabled;
        private Vector3 velocity;

        private void Start() {
            SetEnabled(true);
        }

        public void SetEnabled(bool state) {
            isEnabled = state;
        }

        private void Update() {
            var x = 0.0f;
            var z = 0.0f;
            var jumpPressed = false;
            
            if (isEnabled) {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");
                jumpPressed = Input.GetButtonDown("Jump");
            }

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0) velocity.y = -2f;

            var playerTransform = transform; // reason: repeated property access of built in component
            var move = playerTransform.right * x + playerTransform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (jumpPressed && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}