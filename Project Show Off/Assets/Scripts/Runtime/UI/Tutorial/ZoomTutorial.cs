using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Tutorial {
    public class ZoomTutorial : TutorialSlide {
        public override string TutorialKey => "build_mode_zoom";
        
        [HorizontalLine(color: EColor.Orange, order = -10000), Header("Movement Tutorial", order = -20000)]
        [SerializeField] private float zoomRequired = 250.0f;

        private float totalZoomed;
        private float currentZoomed;

        protected override void Process() {
            var zoomDelta = InputManager.RawZoom;
            if (!Mathf.Approximately(zoomDelta, 0.0f))
                zoomDelta = Mathf.Sign(zoomDelta);
            
            zoomDelta *= 5.0f; //scrollZoomSensitivity TODO: should probably change this to use the actual camera settings
            totalZoomed += Mathf.Abs(Time.deltaTime * zoomDelta * 200.0f); //zoomSpeed TODO: should probably change this to use the actual camera settings
            currentZoomed = Mathf.Lerp(currentZoomed, totalZoomed, Time.deltaTime * 8.0f /*zoomTime TODO: should probably change this to use the actual camera settings*/);
            
            FillAmount = currentZoomed / zoomRequired;
            if (currentZoomed >= zoomRequired)
                FinishTutorial();
        }
    }
}