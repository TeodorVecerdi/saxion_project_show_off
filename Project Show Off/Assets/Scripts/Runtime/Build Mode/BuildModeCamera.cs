using System;
using Cinemachine;
using NaughtyAttributes;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildModeCamera : MonoBehaviour, IEventSubscriber {
        // ReSharper disable InconsistentNaming
        [HorizontalLine(color: EColor.Green, order = 1), Header("General")]
        [SerializeField, OnValueChanged("OnTimeSettingsChanged")]
        private bool unlinkTimeVariables;
        [SerializeField, Required] private CinemachineVirtualCamera virtualCamera;
        [SerializeField, Required] private Camera actualCamera;

        [HorizontalLine(color: EColor.Blue, order = 1), Header("Movement")]
        [SerializeField] private float normalMovementSpeed = 1.0f;
        [SerializeField] private float boostMovementSpeed = 3.0f;
        [OnValueChanged("OnTimeSettingsChanged"), SerializeField]
        private float movementTime = 5.0f;

        [HorizontalLine(color: EColor.Orange, order = 1), Header("Rotation")]
        [SerializeField] private float normalRotationSpeed = 1.0f;
        [SerializeField] private float boostRotationSpeed = 3.0f;
        [ShowIf(nameof(unlinkTimeVariables)), OnValueChanged("OnTimeSettingsChanged"), SerializeField]
        private float rotationTime = 5.0f;

        [HorizontalLine(color: EColor.Red, order = 1), Header("Zoom")]
        [SerializeField] private float normalZoomSpeed = 1.0f;
        [SerializeField] private float boostZoomSpeed = 3.0f;
        [ShowIf(nameof(unlinkTimeVariables)), OnValueChanged("OnTimeSettingsChanged"), SerializeField]
        private float zoomTime = 5.0f;
        [SerializeField] private float minZoom = 40;
        [SerializeField] private float maxZoom = 100;
        // ReSharper restore InconsistentNaming

        // Settings variables
        private float movementSpeed;
        private float zoomSpeed;
        private float rotationSpeed;
        private float actualMovementTime;
        private float actualZoomTime;
        private float actualRotationTime;

        // Working variables
        private Vector3 dragStartPosition;
        private Vector3 dragCurrentPosition;
        private Vector3 rotateStartPosition;
        private Vector3 rotateCurrentPosition;
        private Vector3 newPosition;
        private Vector3 newZoom;
        private Quaternion newRotation;
        
        private Transform cameraTransform;
        private Plane dragPlane;
        private Mouse mouse;
        private bool isEnabled = false;
        private IDisposable gameModeToggleEventUnsubscriber;

        private void Awake() {
            cameraTransform = virtualCamera.transform;
            dragPlane = new Plane(Vector3.up, Vector3.zero);
            mouse = Mouse.current;
            gameModeToggleEventUnsubscriber = EventQueue.Subscribe(this, EventType.GameModeToggle);
            
            DisableBuildMode();
        }

        private void OnEnable() {
            InputSystem.onDeviceChange += OnDeviceChanged;
        }
        
        private void OnDisable() {
            InputSystem.onDeviceChange -= OnDeviceChanged;
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscriber.Dispose();
        }

        private void Update() {
            if (!isEnabled) return;

            if (InputManager.IsBoosting) {
                movementSpeed = boostMovementSpeed;
                rotationSpeed = boostRotationSpeed;
                zoomSpeed = boostZoomSpeed;
            } else {
                movementSpeed = normalMovementSpeed;
                rotationSpeed = normalRotationSpeed;
                zoomSpeed = normalZoomSpeed;
            }
            HandleDragInput();
            HandleInput();
        }

        private void HandleDragInput() {
            if (mouse == null) return;

            var mousePosition = (Vector3) mouse.position.ReadValue();

            // Pan / Move
            if (mouse.leftButton.wasPressedThisFrame) {
                var ray = actualCamera.ScreenPointToRay(mousePosition);
                if (dragPlane.Raycast(ray, out var distance)) {
                    dragStartPosition = ray.GetPoint(distance);
                }
            }

            if (mouse.leftButton.isPressed) {
                var ray = actualCamera.ScreenPointToRay(mousePosition);
                if (dragPlane.Raycast(ray, out var distance)) {
                    dragCurrentPosition = ray.GetPoint(distance);
                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
            }

            // Rotate
            if (mouse.rightButton.wasPressedThisFrame) {
                rotateStartPosition = mousePosition;
            }

            if (mouse.rightButton.isPressed) {
                rotateCurrentPosition = mousePosition;
                var difference = rotateStartPosition - rotateCurrentPosition;
                rotateStartPosition = rotateCurrentPosition;
                newRotation *= Quaternion.Euler(Vector3.up * (-difference.x * 0.1f));
            }
        }

        private void HandleInput() {
            var movementDelta = InputManager.CameraKeyboardPan * Time.deltaTime;
            var rotationDelta = InputManager.Rotation * Time.deltaTime;
            var zoomDelta = InputManager.RawZoom;
            if (!Mathf.Approximately(zoomDelta, 0.0f))
                zoomDelta = Mathf.Sign(zoomDelta);

            newPosition += movementDelta.x * movementSpeed * transform.right + movementDelta.y * movementSpeed * transform.forward;
            newRotation *= Quaternion.Euler(rotationDelta * rotationSpeed * Vector3.up);
            newZoom += zoomDelta * new Vector3(0, -zoomSpeed, zoomSpeed);
            newZoom.y = newZoom.y.Clamped(minZoom, maxZoom);
            newZoom.z = newZoom.z.Clamped(-maxZoom, -minZoom);

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * actualMovementTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * actualRotationTime);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * actualZoomTime);
        }

        private void EnableBuildMode() {
            isEnabled = true;
            var baseTransform = transform; // reason: repeated property access of built in component is inefficient 
            newPosition = baseTransform.position;
            newRotation = baseTransform.rotation;
            newZoom = cameraTransform.localPosition;
            OnTimeSettingsChanged();

            InputManager.BuildModeActions.Enable();
            virtualCamera.Priority = 100;
        }

        private void DisableBuildMode() {
            isEnabled = false;
            InputManager.BuildModeActions.Disable();
            virtualCamera.Priority = 0;
        }

        private void OnTimeSettingsChanged() {
            if (unlinkTimeVariables) {
                actualMovementTime = movementTime;
                actualRotationTime = rotationTime;
                actualZoomTime = zoomTime;
                return;
            }

            actualMovementTime = actualRotationTime = actualZoomTime = movementTime;
        }

        private void ToggleCameraMode() {
            if (isEnabled) DisableBuildMode();
            else EnableBuildMode();
        }
        
        private void OnDeviceChanged(InputDevice device, InputDeviceChange change) {
            mouse = Mouse.current;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    ToggleCameraMode();
                    return false;
                }
                default: return false;
            }
        }
    }
}