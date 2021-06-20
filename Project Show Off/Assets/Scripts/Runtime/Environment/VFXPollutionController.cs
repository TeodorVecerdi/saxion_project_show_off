using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.VFX;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(VisualEffect))]
    public sealed class VFXPollutionController : MonoBehaviour, IEventSubscriber {
        [InfoBox("Effectors get applied top-to-bottom, meaning the last effector will overwrite the first one if they are affecting the same property")]
        [SerializeField] private List<VFXPollutionEffector> effectors;
        private VisualEffect visualEffect;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            visualEffect = GetComponent<VisualEffect>();
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.PollutionUpdate)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PollutionUpdateEvent pollutionUpdateEvent: {
                    foreach (var vfxPollutionEffector in effectors) {
                        vfxPollutionEffector.Apply(visualEffect, pollutionUpdateEvent.RawPollution, pollutionUpdateEvent.PollutionRatio);
                    }

                    return false;
                }
                
                default: return false;
            }
        }
    }
}