using System;
using Runtime.Event;
using TMPro;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class TestPollutionDisplay : MonoBehaviour, IEventSubscriber {
        [SerializeField] private TextMeshProUGUI pollutionText;
        private IDisposable pollutionUpdateEventUnsubscribeToken;
        
        private void Awake() {
            pollutionUpdateEventUnsubscribeToken = this.Subscribe(EventType.PollutionUpdate);
        }

        private void OnDestroy() {
            pollutionUpdateEventUnsubscribeToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PollutionUpdateEvent pollutionUpdateEvent: {
                    pollutionText.text = $"Pollution: {pollutionUpdateEvent.RawPollution:F1} ({pollutionUpdateEvent.PollutionRatio.Clamped01()*100.0f :F1}%)";
                    return false;
                }
                default: return false;
            }
        }
    }
}