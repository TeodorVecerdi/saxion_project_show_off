using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class TrashBin : MonoBehaviour, IEventSubscriber {
        [SerializeField] public float CollectionRadius = 3.0f;
        [SerializeField] private MaterialInventory inventory;

        private bool canCollect;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashSpawn)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void OnTriggerEnter(Collider other) {
            canCollect = true;
        }

        private void OnTriggerExit(Collider other) {
            canCollect = false;
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case TrashEvent {Type: EventType.TrashSpawn} trashEvent: {
                    var spawnPosition = trashEvent.Pickup.transform.position;
                    var squareDistance = (spawnPosition - transform.position).sqrMagnitude;
                    
                    if (squareDistance <= CollectionRadius * CollectionRadius) {
                        inventory.Add(trashEvent.Pickup.TrashPickup.TrashMaterial, trashEvent.Pickup.Mass);
                        
                        EventQueue.QueueEvent(new TrashPickupBinEvent(this, trashEvent.Pickup.TrashPickup, trashEvent.Pickup.Mass));
                        trashEvent.Pickup.DOKill();
                        Destroy(trashEvent.Pickup.gameObject);
                    }

                    return false;
                }
                default: return false;
            }
        }
    }
}