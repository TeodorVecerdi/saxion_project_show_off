using UnityCommons;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookWithMouse : MonoSingleton<LookWithMouse> {
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;

    private bool isEnabled;
    private float xRotation;
    
    public void SetEnabled(bool state) {
        isEnabled = state;
        if (isEnabled) {
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        Cursor.lockState = CursorLockMode.None;
    }

    private void Start() {
        SetEnabled(true);
    }

    private void Update() {
        if(!isEnabled) return;
        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null) {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
        }

        if (Gamepad.current != null) {
            var value = Gamepad.current.rightStick.ReadValue() * 2;
            mouseX += value.x;
            mouseY += value.y;
        }

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}