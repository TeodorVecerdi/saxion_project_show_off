using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PollutionTracker : MonoBehaviour, IEventSubscriber {
        [InfoBox("The amount with which raw pollution gets divided. Basically controls how much pollution is necessary to reach 'max' pollution.\nA value of 100 means 100 pollution = 100% polluted")]
        [SerializeField] private float pollutionMultiplier = 100;
        
        private float rawPollution;
        private List<IDisposable> eventUnsubscribeTokens;

        public float PollutionRatio => rawPollution / pollutionMultiplier;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupSuccess), 
                this.Subscribe(EventType.TrashSpawn),
                this.Subscribe(EventType.PollutionChange)
            };
        }

        private void OnDestroy() {
            foreach (var unsubscribeToken in eventUnsubscribeTokens) {
                unsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void UpdatePollution() {
            EventQueue.QueueEvent(new PollutionUpdateEvent(this, rawPollution, PollutionRatio));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case TrashPickupEvent {Type: EventType.TrashSpawn} trashSpawnEvent: {
                    rawPollution += trashSpawnEvent.Pickup.TrashPickup.PollutionAmount;
                    UpdatePollution();
                    return false;
                }
                case TrashPickupEvent {Type: EventType.TrashPickupSuccess} trashPickupSuccessEvent: {
                    rawPollution -= trashPickupSuccessEvent.Pickup.TrashPickup.PollutionAmount;
                    UpdatePollution();
                    return false;
                }
                case PollutionChangeEvent pollutionChangeEvent: {
                    rawPollution += pollutionChangeEvent.Delta;
                    UpdatePollution();
                    return false;
                }
                default: return false;
            }
        }
    }
}