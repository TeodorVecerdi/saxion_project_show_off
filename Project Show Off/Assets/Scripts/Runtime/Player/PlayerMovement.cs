using Runtime;
using UnityCommons;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private InputAction jump;
    private InputAction movement;
    private Vector3 velocity;

    public void SetEnabled(bool state) {
        isEnabled = state;
    }

    private void Start() {
        SetEnabled(true);

        movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
        movement.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

        jump = new InputAction("PlayerJump", binding: "<Gamepad>/a");
        jump.AddBinding("<Keyboard>/space");

        movement.Enable();
        jump.Enable();
    }

    // Update is called once per frame
    private void Update() {
        var x = 0.0f;
        var z = 0.0f;
        var jumpPressed = false;
        
        if (isEnabled) {
            var delta = movement.ReadValue<Vector2>();
            x = delta.x;
            z = delta.y;
            jumpPressed = Mathf.Approximately(jump.ReadValue<float>(), 1);
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