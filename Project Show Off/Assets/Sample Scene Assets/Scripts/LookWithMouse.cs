using System;
using System.Runtime.InteropServices;
using Runtime;
using UnityEngine;

public class LookWithMouse : MonoBehaviour {
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    
    private Vector2Int screenMiddle;
    private float xRotation = 0f;
    private bool enabled;

    private void Start() {
        // Cursor.visible = false;
        enabled = true;
        screenMiddle = new Vector2Int(Screen.width + Screen.width / 2, Screen.height / 2);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            enabled = !enabled;
        }

        if (!enabled) return;
        
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}