using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class NpcSpawner : MonoBehaviour, IEventSubscriber {
        [SerializeField] private List<NpcAI> NpcPrefabs;
        [SerializeField] private int minNpcCount = 8;
        [SerializeField] private int maxNpcCount = 40;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BarUpdate)
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
                case BarUpdateEvent barUpdateEvent: {
                    UpdateNpcCount(barUpdateEvent.PeopleHappiness);
                    return false;
                }
                default: return false;
            }
        }

        private void UpdateNpcCount(float peopleHappiness) {
            var npcCount = Mathf.FloorToInt(peopleHappiness.Map(0.0f, 1.0f, minNpcCount, maxNpcCount));
        }
    }
}
