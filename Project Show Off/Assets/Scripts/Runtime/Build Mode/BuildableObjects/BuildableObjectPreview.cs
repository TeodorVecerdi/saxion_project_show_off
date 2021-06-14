using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class BuildableObjectPreview : MonoBehaviour, IEventSubscriber {
        [Header("References")]
        [SerializeField] private GameObject objectContainer;

        private IDisposable performBuildEventUnsubscribeToken;
        private List<Collider> colliders;

        private void Awake() {
            performBuildEventUnsubscribeToken = this.Subscribe(EventType.PerformBuild);
        }

        private void Start() {
            colliders = new List<Collider>();
            objectContainer.GetComponentsInChildren(true, colliders);
            foreach (var collider in colliders) {
                collider.enabled = false;
            }
        }
        

        private void OnDestroy() {
            performBuildEventUnsubscribeToken.Dispose();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case PerformBuildEvent performBuildEvent: {
                    foreach (var collider in colliders) {
                        collider.enabled = true;
                    }
                    objectContainer.transform.SetParent(null);
                    objectContainer.name = performBuildEvent.BuildableObject.Name;
                    Destroy(gameObject);
                    return false;
                }
                default: return false;
            }
        }
    }
}