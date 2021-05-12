using UnityEngine;

namespace Runtime {
    public class PlayerMovement : MonoBehaviour {

        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpHeight;

        private CharacterController controller;
        private Vector3 velocity;

        private void Awake() {
            controller = GetComponent<CharacterController>();
        }

        private void Update() {
            var grounded = controller.isGrounded;
            if (grounded && velocity.y < 0.0f) velocity.y = 0.0f;

            var delta = InputManager.PlayerMovement;
            var playerTransform = transform; // reason: repeated property access of built in component
            var movement = playerTransform.right * delta.x + playerTransform.forward * delta.y;
            controller.Move(speed * Time.deltaTime * movement);

            if (InputManager.WasJumpTriggered && grounded) 
                velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}