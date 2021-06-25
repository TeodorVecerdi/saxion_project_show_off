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
        [SerializeField] private float spawnRadius = 5.0f;

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
        
        private void SpawnNpc() {
            var attemptedSpawnPosition = Rand.InsideUnitSphere * spawnRadius + transform.position;
            if (NavMesh.SamplePosition(attemptedSpawnPosition, out var navMeshHit, 10.0f, -1)) {
                var spawnPosition = navMeshHit.position;
                var npc = Instantiate(Rand.ListItem(NpcPrefabs), spawnPosition, Quaternion.Euler(0, Rand.Float * 360.0f, 0), transform);
                npc.DespawnPosition = transform.position;
                npcAIs.Add(npc);
            } else {
                Debug.LogError("Could not find valid spot for NPC spawning");
            }
        }

        private void DespawnNpcs(int amount) {
            var npcsToRemove = new List<NpcAI>();
            foreach (var npcAI in npcAIs) {
                if (npcAI.IsDespawning) continue;
                
                npcAI.Despawn();
                npcsToRemove.Add(npcAI);
                amount--;
                if(amount == 0) break;
            }
            
            foreach (var npcAI in npcsToRemove) {
                npcAIs.Remove(npcAI);
            }

            if (amount != 0) {
                Debug.LogError($"Could not despawn enough NPCs. Remaining {amount}");
            }
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
            Debug.Log($"NPC COUNTS: [{npcCount}] -> [{newNpcCount}]");
            var npcDifference = newNpcCount - npcCount;
            npcCount = newNpcCount;
            if(npcDifference == 0) return;

            if (npcDifference < 0) DespawnNpcs(-npcDifference);
            else {
                for (var i = 0; i < npcDifference; i++) {
                    SpawnNpc();
                }
            }
        }
    }
}
