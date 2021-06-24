using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using UnityEngine.AI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class NpcSpawner : MonoBehaviour, IEventSubscriber {
        [SerializeField] private List<NpcAI> NpcPrefabs;
        [SerializeField] private int minNpcCount = 8;
        [SerializeField] private int maxNpcCount = 40;

        private int npcCount;
        private List<NpcAI> npcAIs;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BarUpdate)
            };
            npcAIs = new List<NpcAI>();
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }
        
        private void SpawnNpcs(int amount) {
            var attemptedSpawnPosition = Rand.InsideUnitSphere * 7.0f + transform.position;
            if (NavMesh.SamplePosition(attemptedSpawnPosition, out var navMeshHit, 10.0f, -1)) {
                
            } else {
                Debug.LogError("Could not find valid spot for NPC");
            }
        }

        private void DespawnNpcs(int amount) {
            throw new NotImplementedException();
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
            var newNpcCount = Mathf.FloorToInt(peopleHappiness.Map(0.0f, 1.0f, minNpcCount, maxNpcCount));
            var npcDifference = newNpcCount - npcCount;
            if(npcDifference == 0) return;

            if (npcDifference < 0) DespawnNpcs(npcDifference);
            else SpawnNpcs(npcDifference);
        }
    }
}
