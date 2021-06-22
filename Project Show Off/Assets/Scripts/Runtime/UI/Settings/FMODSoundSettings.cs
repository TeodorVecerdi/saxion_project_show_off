using System;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class FMODSoundSettings : MonoBehaviour, IEventSubscriber {
        [SerializeField] private BusControlFMOD masterBus;

        private IDisposable unsubscribeEventToken;

        private void Awake() {
            unsubscribeEventToken = this.Subscribe(EventType.SettingsChanged);
        }

        private void Start() {
            masterBus.SetVolume(PlayerPrefs.GetFloat("Settings_SfxVolume", 1.0f));
        }

        private void OnDestroy() {
            unsubscribeEventToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case SettingsChangedEvent settingsChangedEvent: {
                    masterBus.SetVolume(settingsChangedEvent.SfxVolume);
                    return false;
                }
                default: return false;
            }
        }
    }
}