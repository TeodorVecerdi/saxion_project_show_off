using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildableObjectPreview : MonoBehaviour, IEventSubscriber {
        [Header("Settings")]
        [SerializeField] private string objectName;

        [Header("References")]
        [SerializeField] private GameObject objectContainer;

        private IDisposable performBuildEventUnsubscriber;
        private List<Collider> colliders;

        private void Awake() {
            performBuildEventUnsubscriber = EventQueue.Subscribe(this, EventType.PerformBuild);
        }

        private void Start() {
            colliders = new List<Collider>();
            objectContainer.GetComponentsInChildren(true, colliders);
            foreach (var collider in colliders) {
                collider.enabled = false;
            }
        }

        private void OnDestroy() {
            performBuildEventUnsubscriber.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.PerformBuild}: {
                    foreach (var collider in colliders) {
                        collider.enabled = true;
                    }
                    objectContainer.transform.SetParent(null);
                    Destroy(gameObject);
                    return false;
                }
                default: return false;
            }
        }
    }
}