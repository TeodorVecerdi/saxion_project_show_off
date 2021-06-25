using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    [RequireComponent(typeof(CanvasGroup))]
    public class BulldozeConfirmation : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float animationDuration = 0.25f;
        private CanvasGroup canvasGroup;
        
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.GameModeToggle)
            };
            gameObject.SetActive(false);
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }
            eventUnsubscribeTokens.Clear();
        }

        public void Hide() {
            canvasGroup.DOFade(0.0f, animationDuration)
                       .OnComplete(() => gameObject.SetActive(false));
        }

        public void Show() {
            gameObject.SetActive(true);
            canvasGroup.DOFade(1.0f, animationDuration);
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    if (!gameObject.activeSelf) return false;

                    canvasGroup.alpha = 0.0f;
                    gameObject.SetActive(false);
                    
                    return false;
                }
                default: return false;
            }
        }
    }
}