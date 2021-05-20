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
        private List<IDisposable> eventUnsubscribers;
        private RectTransform rectTransform;
        private float width;
        private bool isVisible;
        
        private void Awake() {
            rectTransform = (RectTransform) transform; 
            width = rectTransform.sizeDelta.x;

            eventUnsubscribers = new List<IDisposable>();
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.BeginBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.CancelBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.PerformBuild));
            eventUnsubscribers.Add(EventQueue.Subscribe(this, EventType.GameModeToggle));
        }

        private void OnDestroy() {
            foreach (var eventUnsubscriber in eventUnsubscribers) {
                eventUnsubscriber.Dispose();
            }
            eventUnsubscribers.Clear();
        }

        private void Start() {
            
            buildableObjects = new List<BuildableObject>(Resources.LoadAll<BuildableObject>("Buildable Objects"));
            Debug.Log($"Creating {buildableObjects.Count} entries");
            foreach (var buildableObject in buildableObjects) {
                var buildableEntry = Instantiate(buildableEntryPrefab, buildableEntryContainer);
                buildableEntry.BuildUI(buildableObject);
            }
        }

        private void SetViewVisible(bool visible) {
            if(isVisible == visible) return;
            Debug.Log($"Setting view visible {visible}");
            isVisible = visible;
            rectTransform.DOKill();
            rectTransform.DOAnchorPosX(isVisible ? 0.0f : -width, transitionDuration);
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent _: 
                case EmptyEvent {Type:EventType.GameModeToggle}: {
                    SetViewVisible(!isVisible);
                    return false;
                }
                case EmptyEvent {Type: EventType.CancelBuild}:
                case EmptyEvent {Type: EventType.PerformBuild}: {
                    SetViewVisible(true);
                    return false;
                }
                default: return false;
            }
        }
    }
}