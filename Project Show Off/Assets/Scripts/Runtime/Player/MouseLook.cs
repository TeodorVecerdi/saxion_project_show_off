using System;
using Cinemachine;
using UnityCommons;
using UnityEngine;

namespace Runtime {
    public class MouseLook : CinemachineExtension {
        [SerializeField] private float clampAngle = 80.0f;
        [SerializeField] private float mouseSensitivity = 400.0f;

        private Vector3 startingRotation;

        protected override void Awake() {
            startingRotation = transform.localEulerAngles;
            base.Awake();
        }

        private void Start() {
            SetMouseLock(true);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
            if(!vcam.Follow || stage != CinemachineCore.Stage.Aim || !Application.isPlaying) return;
            var input = InputManager.MouseDelta * Time.deltaTime * mouseSensitivity * 0.06666667f;
            startingRotation.x += input.x;
            startingRotation.y += input.y;
            startingRotation.y = startingRotation.y.Clamped(-clampAngle, clampAngle);
            state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0.0f);
        }
        
        internal static void SetMouseLock(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}

