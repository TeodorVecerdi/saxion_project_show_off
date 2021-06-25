using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Runtime.Event;
using TMPro;
using UnityCommons;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreUI : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float animationDuration = 0.25f;
        
        private int score;
        private TextMeshProUGUI text;
        private List<IDisposable> eventUnsubscribeTokens;

        public int Score => score;

        private void Awake() {
            text = GetComponent<TextMeshProUGUI>();

            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.TrashPickupSuccess),
                this.Subscribe(EventType.TrashPickupBin),
                this.Subscribe(EventType.PerformBuild)
            };
        }

        private void Start() {
            text.text = score.ToString();
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        private void AddScore(int scoreAmount) {
            score += scoreAmount;
            UpdateScore();
        }

        private void UpdateScore() {
            text.rectTransform.DOKill(true);
            text.rectTransform.DOPunchScale(Vector3.one * 0.5f, animationDuration);
            Run.After(animationDuration * 0.5f, () => text.text = score.ToString());
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PerformBuildEvent performBuildEvent: {
                    AddScore(performBuildEvent.BuildableObject.BuildScore);
                    return false;
                }
                case TrashEvent {Type: EventType.TrashPickupSuccess} trashPickupSuccessEvent: {
                    AddScore(trashPickupSuccessEvent.Pickup.TrashPickup.PickupScore);
                    return false;
                }
                case TrashPickupBinEvent trashPickupBinEvent: {
                    AddScore(trashPickupBinEvent.TrashPickup.PickupScore);
                    return false;
                }
                default: return false;
            }
        }
    }
}