using System;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class GeneralInput : MonoBehaviour, IEventSubscriber {
        private static bool isBuildModeActive;

        private IDisposable gameModeChangeEventUnsubscribeToken;

        public static bool IsBuildModeActive => isBuildModeActive;

        private void Awake() {
            gameModeChangeEventUnsubscribeToken = this.Subscribe(EventType.GameModeChange);
            isBuildModeActive = false;
        }

        private void OnDestroy() {
            gameModeChangeEventUnsubscribeToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeChange}: {
                    isBuildModeActive = !isBuildModeActive;
                    EventQueue.QueueEvent(new ChangeMouseLockEvent(this, !isBuildModeActive));
                    return false;
                }
                default: return false;
            }
        }
    }
}