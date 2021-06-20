using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.Rendering;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class PollutionVolumeTransition : MonoBehaviour, IEventSubscriber {
        [SerializeField] private Volume dirtyVolume;
        [SerializeField] private AnimationCurve pollutionCurve;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.PollutionUpdate)
            };
        }

        private void OnDestroy() {
            foreach (var unsubscribeToken in eventUnsubscribeTokens) {
                unsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void UpdatePollution(float pollution) {
            dirtyVolume.weight = pollutionCurve.Evaluate(pollution.Clamped01());
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PollutionUpdateEvent pollutionUpdateEvent: {
                    UpdatePollution(pollutionUpdateEvent.PollutionRatio);
                    return false;
                }
                default: return false;
            }
        }
    }
}

