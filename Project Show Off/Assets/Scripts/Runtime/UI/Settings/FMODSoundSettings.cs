using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class FMODSoundSettings : MonoBehaviour, IEventSubscriber {
        [SerializeField] private List<BusControlFMOD> sfxBusses;
        [SerializeField] private List<BusControlFMOD> musicBusses;

        private IDisposable unsubscribeEventToken;

        private void Awake() {
            unsubscribeEventToken = this.Subscribe(EventType.SettingsChanged);
        }

        private void Start() {
            sfxBusses.ForEach(bus => bus.SetVolume(PlayerPrefs.GetFloat("Settings_SfxVolume", 1.0f)));
            musicBusses.ForEach(bus => bus.SetVolume(PlayerPrefs.GetFloat("Settings_MusicVolume", 1.0f)));
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
                    sfxBusses.ForEach(bus => bus.SetVolume(settingsChangedEvent.SfxVolume));
                    musicBusses.ForEach(bus => bus.SetVolume(settingsChangedEvent.MusicVolume));
                    return false;
                }
                default: return false;
            }
        }
    }
}