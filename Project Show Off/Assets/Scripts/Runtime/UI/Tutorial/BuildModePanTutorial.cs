using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Tutorial {
    public class BuildModePanTutorial : TutorialSlide {
        public override string TutorialKey => "build_mode_pan";
        
        [HorizontalLine(color: EColor.Orange, order = -10000), Header("Build Mode Pan/Move Tutorial", order = -20000)]
        [SerializeField] private float movementRequired = 100.0f;

        private new Camera camera;
        private float totalMoved;
        private Vector3 dragStartPosition;
        private Vector3 dragCurrentPosition;
        private Plane dragPlane;

        protected override void OnAwake() {
            camera = ResourcesProvider.MainCamera;
            dragPlane = new Plane(Vector3.up, Vector3.zero);
        }

        protected override void Process() {
            if(!GeneralInput.IsBuildModeActive) return;
            
            var mouse = Mouse.current;
            var mousePosition = (Vector3) mouse.position.ReadValue();

            // Pan / Move
            if (mouse.leftButton.wasPressedThisFrame) {
                var ray = camera.ScreenPointToRay(mousePosition);
                if (dragPlane.Raycast(ray, out var distance)) {
                    dragStartPosition = ray.GetPoint(distance);
                }
            }

            if (mouse.leftButton.isPressed) {
                var ray = camera.ScreenPointToRay(mousePosition);
                if (dragPlane.Raycast(ray, out var distance)) {
                    dragCurrentPosition = ray.GetPoint(distance);
                    totalMoved += (dragStartPosition - dragCurrentPosition).sqrMagnitude;
                }
            }

            var realRequirement = movementRequired * movementRequired;
            FillAmount = totalMoved / realRequirement;
            if(totalMoved >= realRequirement)
                FinishTutorial();
        }
    }
}