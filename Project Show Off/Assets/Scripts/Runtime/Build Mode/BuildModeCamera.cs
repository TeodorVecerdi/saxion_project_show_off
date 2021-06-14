using System;
using Cinemachine;
using NaughtyAttributes;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.InputSystem;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(CameraBoundaries))]
    public class BuildModeCamera : MonoBehaviour, IEventSubscriber {
        // ReSharper disable InconsistentNaming
        [HorizontalLine(color: EColor.Green, order = 1), Header("General")]
        [SerializeField, OnValueChanged("OnTimeSettingsChanged")]
        private bool unlinkTimeVariables;
        [SerializeField, Required] private CinemachineVirtualCamera virtualCamera;
        [SerializeField, Required] private BuildModeVolume buildModeVolume;

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
        [SerializeField] private float scrollZoomSensitivity = 5.0f;
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
        private CameraBoundaries cameraBoundaries;
        private Plane dragPlane;
        private Mouse mouse;
        private IDisposable gameModeToggleEventUnsubscribeToken;

        private void Awake() {
            cameraTransform = virtualCamera.transform;
            cameraBoundaries = GetComponent<CameraBoundaries>();
            dragPlane = new Plane(Vector3.up, Vector3.zero);
            mouse = Mouse.current;
            gameModeToggleEventUnsubscribeToken = this.Subscribe(EventType.GameModeToggle);
            
            DisableBuildMode();
        }

        private void OnEnable() {
            InputSystem.onDeviceChange += OnDeviceChanged;
        }
        
        private void OnDisable() {
            InputSystem.onDeviceChange -= OnDeviceChanged;
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscribeToken.Dispose();
        }

        private void Update() {
            if (!GeneralInput.IsBuildModeActive || !InputManager.BuildModeActions.enabled) return;

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
                var ray = ResourcesProvider.MainCamera.ScreenPointToRay(mousePosition);
                if (dragPlane.Raycast(ray, out var distance)) {
                    dragStartPosition = ray.GetPoint(distance);
                }
            }

            if (mouse.leftButton.isPressed) {
                var ray = ResourcesProvider.MainCamera.ScreenPointToRay(mousePosition);
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
            var zoomDelta = InputManager.RawZoom;
            if (!Mathf.Approximately(zoomDelta, 0.0f))
                zoomDelta = Mathf.Sign(zoomDelta);
            zoomDelta *= scrollZoomSensitivity;
            zoomDelta += InputManager.KeyboardZoom;

            newPosition += movementDelta.x * movementSpeed * transform.right + movementDelta.y * movementSpeed * transform.forward;
            newZoom += Time.deltaTime * zoomDelta * new Vector3(0, -zoomSpeed, zoomSpeed);
            newZoom.y = newZoom.y.Clamped(minZoom, maxZoom);
            newZoom.z = newZoom.z.Clamped(-maxZoom, -minZoom);

            // Limit to boundaries
            newPosition.x = newPosition.x.Clamped(cameraBoundaries.MinimumPosition.x, cameraBoundaries.MaximumPosition.x);
            newPosition.z = newPosition.z.Clamped(cameraBoundaries.MinimumPosition.z, cameraBoundaries.MaximumPosition.z);

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * actualMovementTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * actualRotationTime);
            
            //!! reason: repeated property access of built in component is inefficient
            var cameraLocalPosition = cameraTransform.localPosition;
            cameraLocalPosition = Vector3.Lerp(cameraLocalPosition, newZoom, Time.deltaTime * actualZoomTime);
            buildModeVolume.UpdateShadowDistance(cameraLocalPosition.y);
            cameraTransform.localPosition = cameraLocalPosition;
        }

        private void EnableBuildMode() {
            //!! reason: repeated property access of built in component is inefficient
            var baseTransform = transform;  
            newPosition = baseTransform.position;
            newRotation = baseTransform.rotation;
            newZoom = cameraTransform.localPosition;
            OnTimeSettingsChanged();

            virtualCamera.Priority = 100;
        }

        private void DisableBuildMode() {
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
            if (GeneralInput.IsBuildModeActive) EnableBuildMode();
            else DisableBuildMode();
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