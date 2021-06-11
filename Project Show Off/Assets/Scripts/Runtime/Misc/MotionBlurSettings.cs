using System;
using NaughtyAttributes;
using Runtime.Event;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class MotionBlurSettings : MonoBehaviour, IEventSubscriber {
        [SerializeField, Required] private Volume globalSettingsVolume;

        private MotionBlur motionBlurComponent;
        private IDisposable settingsChangedEventUnsubscribeToken;
        
        private void Awake() {
            if (globalSettingsVolume.profile.TryGet<MotionBlur>(out var motionBlur)) {
                settingsChangedEventUnsubscribeToken = this.Subscribe(EventType.SettingsChanged);
                motionBlurComponent = motionBlur;
                return;
            }

            
            // self-destroy if couldn't get motion blur component
            Destroy(gameObject);
        }

        private void Start() {
            UpdateMotionBlur(PlayerPrefs.GetInt("Settings_MotionBlur", 1) == 1);
        }

        private void OnDestroy() {
            settingsChangedEventUnsubscribeToken?.Dispose();
        }

        private void UpdateMotionBlur(bool enableMotionBlur) {
            motionBlurComponent.intensity.value = enableMotionBlur ? 1.0f : 0.0f;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case SettingsChangedEvent settingsChangedEvent: {
                    UpdateMotionBlur(settingsChangedEvent.EnableMotionBlur);
                    return false;
                }
                
                default: return false;
            }
        }
    }
}