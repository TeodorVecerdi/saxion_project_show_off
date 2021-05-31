using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class PollutionTracker : MonoBehaviour, IEventSubscriber {

        private float pollution;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupSuccess), 
                this.Subscribe(EventType.TrashSpawn)
            };
        }

        private void OnDestroy() {
            foreach (var unsubscribeToken in eventUnsubscribeTokens) {
                unsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void UpdatePollution() {
            EventQueue.QueueEvent(new PollutionUpdateEvent(this, pollution));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case TrashPickupEvent {Type: EventType.TrashSpawn} trashSpawnEvent: {
                    pollution += trashSpawnEvent.Pickup.Item.PollutionAmount;
                    UpdatePollution();
                    return false;
                }
                case TrashPickupEvent {Type: EventType.TrashPickupSuccess} trashPickupSuccessEvent: {
                    pollution -= trashPickupSuccessEvent.Pickup.Item.PollutionAmount;
                    UpdatePollution();
                    return false;
                }
                default: return false;
            }
        }
    }
}