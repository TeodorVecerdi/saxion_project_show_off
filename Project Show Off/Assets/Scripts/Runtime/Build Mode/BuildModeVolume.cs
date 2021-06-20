using System;
using Runtime.Event;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class BuildModeVolume : MonoBehaviour, IEventSubscriber {
        private Volume volume;
        private HDShadowSettings shadowSettings;
        private IDisposable gameModeToggleEventUnsubscribeToken;

        [SerializeField] private float shadowDistanceMultiplier = 20;
#if UNITY_EDITOR
        //debug:
        private float originalShadowDistance;
#endif
        private const float sqrt2 = 1.4142135623731f;

        private void Awake() {
            gameModeToggleEventUnsubscribeToken = this.Subscribe(EventType.GameModeToggle);
            volume = GetComponent<Volume>();
            volume.weight = 0.0f;
            volume.profile.TryGet(out shadowSettings);
#if UNITY_EDITOR
            originalShadowDistance = shadowSettings.maxShadowDistance.value;
#endif
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscribeToken.Dispose();
#if UNITY_EDITOR
            //debug: Reset shadow distance when exiting scene
            shadowSettings.maxShadowDistance.value = originalShadowDistance;
#endif
        }

        public void UpdateShadowDistance(float zoom) {
            // 45 deg angle 
            var distanceToCenter = zoom * sqrt2;
            var shadowDistance = distanceToCenter * shadowDistanceMultiplier;
            shadowSettings.maxShadowDistance.value = shadowDistance;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    volume.weight = GeneralInput.IsBuildModeActive ? 1.0f : 0.0f;
                    return false;
                }
                default: return false;
            }
        }
    }
}