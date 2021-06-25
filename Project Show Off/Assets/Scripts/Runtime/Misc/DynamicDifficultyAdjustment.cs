using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Runtime.Event;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class DynamicDifficultyAdjustment : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float adjustmentFrequency = 5.0f;
        [SerializeField] private float majorAdjustmentFrequency = 60.0f;
        
        [ShowNonSerializedField] private float currentDifficulty;
        [ShowNonSerializedField] private float extraDifficultyChange;
        [ShowNonSerializedField] private int totalSpawned;
        [ShowNonSerializedField] private int totalPickedUp;
        [ShowNonSerializedField] private int spawned, pickedUp;
        [ShowNonSerializedField] private float adjustmentTimer;
        [ShowNonSerializedField] private float majorAdjustmentTimer;

        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupBin),
                this.Subscribe(EventType.TrashPickupSuccess),
                this.Subscribe(EventType.TrashSpawn)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void Start() {
            pickedUp = totalPickedUp = spawned = totalSpawned = 0;
            currentDifficulty = 0.0f;
        }

        private void Update() {
            adjustmentTimer += Time.deltaTime;
            majorAdjustmentTimer += Time.deltaTime;

            if (adjustmentTimer >= adjustmentFrequency) {
                adjustmentTimer -= adjustmentFrequency;
                Adjust();
            }

            if (majorAdjustmentTimer >= majorAdjustmentFrequency) {
                majorAdjustmentTimer -= majorAdjustmentFrequency;
                AdjustMajor();
            }
        }

        private void Adjust() {
            if (spawned == 0 || pickedUp == 0) {
                spawned++;
                pickedUp++;
            }
            var ratio = 0.1f * (pickedUp / (float) spawned);
            
            if (ratio < 1.0f) {
                currentDifficulty -= ratio;
            } else if (ratio >= 1.0f) {
                currentDifficulty += ratio;
            }

            pickedUp = 0;
            spawned = 0;
            currentDifficulty = currentDifficulty.Clamped(-1.0f, 1.0f);
            EventQueue.QueueEvent(new DifficultyAdjustmentEvent(this, currentDifficulty + extraDifficultyChange));
        }

        private void AdjustMajor() {
            if (totalPickedUp == 0 || totalSpawned == 0) {
                totalSpawned++;
                totalPickedUp++;
            } 
            var ratio = totalPickedUp / (float) totalSpawned;
            if (ratio < 1.0f) {
                extraDifficultyChange -= ratio;
            } else if (ratio > 1.0f) {
                extraDifficultyChange += ratio;
            }

            totalPickedUp = 0;
            totalSpawned = 0;
            extraDifficultyChange = extraDifficultyChange.Clamped(-1.0f, 1.0f);
            EventQueue.QueueEvent(new DifficultyAdjustmentEvent(this, currentDifficulty + extraDifficultyChange));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case TrashPickupBinEvent pickupBinEvent: {
                    totalPickedUp++;
                    pickedUp++;
                    return false;
                }
                case TrashEvent {Type: EventType.TrashSpawn} trashSpawnEvent: {
                    totalSpawned++;
                    spawned++;
                    return false;
                }
                case TrashEvent {Type: EventType.TrashPickupSuccess} trashPickupEvent: {
                    totalPickedUp++;
                    pickedUp++;
                    return false;
                }
                default: return false;
            }
        }
    }
}