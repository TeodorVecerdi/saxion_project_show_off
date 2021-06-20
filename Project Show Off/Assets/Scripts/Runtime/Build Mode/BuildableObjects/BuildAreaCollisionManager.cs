using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Event;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class BuildAreaCollisionManager : MonoBehaviour, IEventSubscriber {
        private List<MeshCollider> inactiveColliders;
        private List<MeshCollider> activeColliders;
        private List<IDisposable> eventUnsubscribeTokens;

        private void Awake() {
            inactiveColliders = new List<MeshCollider>();
            activeColliders = new List<MeshCollider>();
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.BeginBuild),
                this.Subscribe(EventType.CancelBuild),
                this.Subscribe(EventType.PerformBuild),
            };
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }

            eventUnsubscribeTokens.Clear();
        }

        private void Load(List<BuildArea> buildAreas) {
            if (inactiveColliders.Count < buildAreas.Count) {
                ExpandPool(buildAreas.Count - inactiveColliders.Count);
            }

            for (var i = 0; i < buildAreas.Count; i++) {
                // Get pooled
                if (buildAreas[i].BakedMesh == null) continue;
                var currentCollider = inactiveColliders[0];
                inactiveColliders.RemoveAt(0);
                activeColliders.Add(currentCollider);

                currentCollider.sharedMesh = buildAreas[i].BakedMesh;
                currentCollider.gameObject.SetActive(true);
            }
        }

        private void Unload() {
            while (activeColliders.Count > 0) {
                inactiveColliders.Add(activeColliders[0]);
                activeColliders[0].sharedMesh = null;
                activeColliders[0].gameObject.SetActive(false);
                activeColliders.RemoveAt(0);
            }
        }

        private void ExpandPool(int count) {
            for (var i = 0; i < count; i++) {
                inactiveColliders.Add(MakePooledObject());
            }
        }

        private MeshCollider MakePooledObject() {
            return new GameObject($"Collider[{inactiveColliders.Count + activeColliders.Count}]") {layer = LayerMask.NameToLayer("Build Area"), transform = { parent = transform }}.AddComponent<MeshCollider>();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case BeginBuildEvent beginBuildEvent: {
                    Load(beginBuildEvent.BuildableObject.BuildAreas);
                    return false;
                }
                case PerformBuildEvent performBuildEvent:
                case EmptyEvent {Type: EventType.CancelBuild}: {
                    Unload();
                    return false;
                }
                default: return false;
            }
        }
    }
}