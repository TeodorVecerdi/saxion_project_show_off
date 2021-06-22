using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using EventType = Runtime.Event.EventType;
using Object = UnityEngine.Object;

namespace Runtime {
    public sealed class PollutionEffectorController : MonoBehaviour, IEventSubscriber {
        [InfoBox("Effectors get applied top-to-bottom, meaning the last effector will overwrite the first one if they are affecting the same property")]
        [SerializeField] private List<EffectorData> effectors2;
        
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.PollutionUpdate)
            };

            foreach (var effectorData in effectors2) {
                effectorData.ExtraData = new Dictionary<string, object>();
            }
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
                    Object lastValidTarget = null;
                    foreach (var pollutionEffector in effectors2) {
                        var currentTarget = pollutionEffector.Target;
                        if (pollutionEffector.GetLastTarget) currentTarget = lastValidTarget != null ? lastValidTarget : pollutionEffector.Target;
                        else lastValidTarget = currentTarget;
                        
                        pollutionEffector.Effector.Apply(currentTarget, pollutionEffector.ExtraData, pollutionUpdateEvent.RawPollution, pollutionUpdateEvent.PollutionRatio);
                    }

                    return false;
                }
                
                default: return false;
            }
        }

        [Serializable]
        public class EffectorData {
            public PollutionEffector Effector;
            public Object Target;
            public bool GetLastTarget;
            public Dictionary<string, object> ExtraData;
        }
    }
}