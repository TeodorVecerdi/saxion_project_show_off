using System;
using System.Collections.Generic;
using Runtime.Event;
using UnityEngine;
using UnityEngine.AI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class BuildableObjectPreview : MonoBehaviour, IEventSubscriber {
        [Header("References")]
        [SerializeField] private GameObject directionIndicator;

        private IDisposable performBuildEventUnsubscribeToken;
        private List<Collider> colliders;
        private List<NavMeshObstacle> obstacles;

        private void Awake() {
            performBuildEventUnsubscribeToken = this.Subscribe(EventType.PerformBuild);
        }

        private void Start() {
            colliders = new List<Collider>();
            gameObject.GetComponentsInChildren(true, colliders);
            colliders.AddRange(gameObject.GetComponents<Collider>());
            foreach (var collider in colliders) {
                collider.enabled = false;
            }

            obstacles = new List<NavMeshObstacle>();
            gameObject.GetComponentsInChildren(true, obstacles);
            obstacles.AddRange(gameObject.GetComponents<NavMeshObstacle>());
            foreach (var navMeshObstacle in obstacles) {
                navMeshObstacle.enabled = false;
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
                    foreach (var navMeshObstacle in obstacles) {
                        navMeshObstacle.enabled = true;
                    }
                    Destroy(directionIndicator);
                    Destroy(this);
                    gameObject.name = performBuildEvent.BuildableObject.Name;
                    return false;
                }
                default: return false;
            }
        }
    }
}