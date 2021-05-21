using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class UIManager : MonoBehaviour, IEventSubscriber {
        [SerializeField] private List<GameObject> gameModeObjects;
        [SerializeField] private List<GameObject> buildModeObjects;
        
        private IDisposable gameModeToggleEventUnsubscribeToken;
        private bool isBuildModeActive;

        private void Awake() {
            gameModeToggleEventUnsubscribeToken = EventQueue.Subscribe(this, EventType.GameModeToggle);
        }

        private void Start() {
            DisableBuildModeUI();
        }

        private void OnDestroy() {
            gameModeToggleEventUnsubscribeToken.Dispose();
        }

        private void EnableBuildModeUI() {
            gameModeObjects.ForEach(obj => obj.SetActive(false));
            buildModeObjects.ForEach(obj => obj.SetActive(true));
        }

        private void DisableBuildModeUI() {
            gameModeObjects.ForEach(obj => obj.SetActive(true));
            buildModeObjects.ForEach(obj => obj.SetActive(false));
        }

        private void OnGameModeToggle() {
            isBuildModeActive = !isBuildModeActive;
            if (isBuildModeActive) EnableBuildModeUI();
            else DisableBuildModeUI();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    OnGameModeToggle();
                    return false;
                }
                default: return false;
            }
        }
    }
}