using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Tutorial {
    public class BuildModeRotateTutorial : TutorialSlide {
        public override string TutorialKey => "build_mode_rotate";
        
        [HorizontalLine(color: EColor.Orange, order = -10000), Header("Build Mode Rotate Tutorial", order = -20000)]
        [SerializeField] private float rotationRequired = 90.0f;

        private float totalRotated;
        private Vector3 dragStartPosition;
        private Vector3 dragCurrentPosition;

        protected override void Process() {
            var mouse = Mouse.current;
            var mousePosition = (Vector3) mouse.position.ReadValue();

            if (mouse.rightButton.wasPressedThisFrame) {
                dragStartPosition = mousePosition;
            }

            if (mouse.rightButton.isPressed) {
                dragCurrentPosition = mousePosition;
                var difference = dragStartPosition - dragCurrentPosition;
                dragStartPosition = dragCurrentPosition;
                totalRotated += Mathf.Abs(difference.x * 0.1f);
            }

            FillAmount = totalRotated / rotationRequired;
            if(totalRotated >= rotationRequired)
                FinishTutorial();
        }
    }
}