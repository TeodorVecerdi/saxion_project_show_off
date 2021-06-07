using System;
using Cinemachine;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class MouseLook : CinemachineExtension, IEventSubscriber {
        [SerializeField] private float clampAngle = 80.0f;
        [SerializeField] private float mouseSensitivity = 400.0f;
        [SerializeField] private Transform lookForwardTransform;

        private Vector3 startingRotation;
        private IDisposable changeMouseLockEventUnsubscribeToken;

        protected override void Awake() {
            startingRotation = lookForwardTransform.eulerAngles;
            changeMouseLockEventUnsubscribeToken = this.Subscribe(EventType.ChangeMouseLock);
            base.Awake();
        }

        private void Start() {
            SetMouseLock(true);
        }

        protected override void OnDestroy() {
            changeMouseLockEventUnsubscribeToken.Dispose();
            base.OnDestroy();
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
            if(!vcam.Follow || stage != CinemachineCore.Stage.Aim || !Application.isPlaying) return;
            var input = InputManager.MouseDelta * deltaTime * mouseSensitivity * 0.06666667f;
            
            startingRotation.x += input.x;
            startingRotation.y += input.y;
            startingRotation.y = startingRotation.y.Clamped(-clampAngle, clampAngle);
            
            lookForwardTransform.rotation = state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0.0f);
        }
        
        private void SetMouseLock(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case ChangeMouseLockEvent mouseLockEvent: {
                    SetMouseLock(mouseLockEvent.State);
                    return false;
                }
                default: return false;
            }
        }
    }
}

