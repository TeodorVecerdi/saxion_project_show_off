using System;
using Runtime.Event;
using UnityEngine;
using UnityEngine.Rendering;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildModeVolume : MonoBehaviour, IEventSubscriber {
        private Volume volume;
        private IDisposable gameModeToggleEventUnsubscribeToken;
        private bool isBuildMode;

        private void Awake() {
            gameModeToggleEventUnsubscribeToken = this.Subscribe(EventType.GameModeToggle);
            volume = GetComponent<Volume>();
            volume.weight = 0.0f;
            isBuildMode = false;
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscribeToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    isBuildMode = !isBuildMode;
                    volume.weight = isBuildMode ? 1.0f : 0.0f;
                    return false;
                }
                default: return false;
            }
        }
    }
}