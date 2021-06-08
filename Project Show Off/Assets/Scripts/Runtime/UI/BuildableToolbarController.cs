using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildableToolbarController : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private float transitionDuration = 0.25f;
        [Header("References")]
        [SerializeField] private Transform buildableEntryContainer;
        [SerializeField] private BuildableEntry buildableEntryPrefab;

        private List<BuildableObject> buildableObjects;
        private List<BuildableEntry> entries;
        private List<IDisposable> eventUnsubscribeTokens;
        private RectTransform rectTransform;
        private float width;
        private bool isVisible;

        private void Awake() {
            rectTransform = (RectTransform) transform;
            width = rectTransform.sizeDelta.x;
            buildableObjects = new List<BuildableObject>(Resources.LoadAll<BuildableObject>("Buildable Objects"));

            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BeginBuild),
                this.Subscribe(EventType.CancelBuild),
                this.Subscribe(EventType.PerformBuild),
                this.Subscribe(EventType.GameModeToggle),
                this.Subscribe(EventType.DepositInventoryResponse)
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }

            eventUnsubscribeTokens.Clear();
        }

        private void Start() {
            entries = new List<BuildableEntry>();
            
            rectTransform.DOKill();
            rectTransform.DOAnchorPosX(isVisible ? 0.0f : -width, transitionDuration);
            
            foreach (var buildableObject in buildableObjects) {
                var buildableEntry = Instantiate(buildableEntryPrefab, buildableEntryContainer);
                entries.Add(buildableEntry);
                buildableEntry.LoadUI(buildableObject);
            }
        }

        private void SetViewVisible(bool visible) {
            if (isVisible == visible) return;
            isVisible = visible;
            rectTransform.DOKill();
            rectTransform.DOAnchorPosX(isVisible ? 0.0f : -width, transitionDuration);
            
            if (!isVisible) return;
            
            foreach (var buildableEntry in entries) {
                buildableEntry.SetEnabled(false);
            }

            EventQueue.QueueEvent(new EmptyEvent(this, EventType.DepositInventoryRequest));
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent _:
                case EmptyEvent {Type: EventType.GameModeToggle}: {
                    SetViewVisible(!isVisible);
                    return false;
                }
                case EmptyEvent {Type: EventType.CancelBuild}:
                case PerformBuildEvent _: {
                    SetViewVisible(true);
                    return false;
                }
                case DepositInventoryResponseEvent inventoryResponseEvent: {
                    var inventory = inventoryResponseEvent.Inventory;
                    foreach (var buildableEntry in entries) {
                        if(!inventory.Contains(buildableEntry.Requirements)) continue;
                        buildableEntry.SetEnabled(true);
                    }
                    return false;
                }
                default: return false;
            }
        }
    }
}