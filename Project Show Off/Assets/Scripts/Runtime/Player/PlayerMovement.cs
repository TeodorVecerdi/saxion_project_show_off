using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PlayerMovement : MonoBehaviour, IEventSubscriber {

        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpHeight;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isEnabled;

        private void Awake() {
            controller = GetComponent<CharacterController>();
        }

        private void Update() {
            var grounded = controller.isGrounded;
            if (grounded && velocity.y < 0.0f) velocity.y = 0.0f;

            var delta = speed * Time.deltaTime * InputManager.PlayerMovement;
            var movement = cameraTransform.right * delta.x + cameraTransform.forward * delta.y;
            movement.y = 0.0f;
            controller.Move(movement);

            if (InputManager.WasJumpTriggered && grounded) 
                velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    ToggleEnabledState();
                    return false;
                }
                default: return false;
            }
        }

        private void ToggleEnabledState() {
            isEnabled = !isEnabled;
            if(isEnabled) InputManager.PlayerActions.Enable();
            else InputManager.PlayerActions.Disable();
        }
    }
}