using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
using Runtime.Event;
using UnityCommons;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public class NpcAI : MonoBehaviour, IEventSubscriber {
        [SerializeField] private float wanderTime = 3.0f;
        [SerializeField] private float wanderDistance = 10.0f;
        [Space]
        [SerializeField] private float trashBinSearchDistance = 15.0f;
        [Space]
        [SerializeField] private Vector2 minMaxTrashWaitTime = new Vector2(2.0f, 5.0f);
        [SerializeField] private Vector2 minMaxTrashWaitTime2 = new Vector2(1.0f, 3.0f);

        /*debug:*/
        [ShowNativeProperty, UsedImplicitly] private string Status => aiState.ToString();
        [ShowNonSerializedField] private static bool debug;

        private NavMeshAgent agent;

        private AIState aiState;
        private bool shouldChangeState;
        private float wanderTimer;

        private int trashQueue;

        private List<IDisposable> eventUnsubscribeTokens;

        [Button, UsedImplicitly]
        private void ToggleDebug() {
            debug = !debug;
        }

        private void Awake() {
            eventUnsubscribeTokens = new List<IDisposable> {
                this.Subscribe(EventType.NpcThrowTrash)
            };

            agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            shouldChangeState = true;
            wanderTimer = Rand.Float * (0.7f * wanderTime);
        }

        private void OnDestroy() {
            foreach (var eventUnsubscribeToken in eventUnsubscribeTokens) {
                eventUnsubscribeToken.Dispose();
            }

            eventUnsubscribeTokens.Clear();
        }

        private void Update() {
            if (shouldChangeState) {
                shouldChangeState = false;
                StartCoroutine(GetNextState());
            }
        }

        private IEnumerator GetNextState() {
            if (aiState == AIState.ApproachBin) {
                aiState = AIState.ThrowTrashBin;
                return ThrowTrash(false);
            }

            if (aiState != AIState.ApproachBin && trashQueue > 0) {
                agent.isStopped = true;

                if (TrashBinInRadius(out var trashBinPosition)) {
                    aiState = AIState.ApproachBin;
                    return ApproachBin(trashBinPosition);
                }

                aiState = AIState.ThrowTrashGround;
                return ThrowTrash(true);
            }

            aiState = AIState.Wander;
            return Wander();
        }

        private IEnumerator Wander() {
            while (true) {
                var stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.25f);
                var newLocation = Rand.InsideUnitSphere * wanderDistance;

                NavMesh.SamplePosition(newLocation + transform.position, out var navMeshHit, wanderDistance, -1);
                agent.SetDestination(navMeshHit.position);

                if (debug) Debug.Log($"Chose new wander position {navMeshHit.position}");

                yield return new WaitUntil(() => {
                    wanderTimer += Time.deltaTime;
                    if (shouldChangeState) return true;
                    if (wanderTimer >= wanderTime) {
                        wanderTimer = 0.0f;
                        return true;
                    }

                    return agent.remainingDistance <= stoppingDistance;
                });
                if (shouldChangeState) yield break;
            }
        }

        private IEnumerator ApproachBin(Vector3 trashBinPosition) {
            //TODO: Speed up (agent + animator)
            NavMesh.SamplePosition(trashBinPosition, out var navMeshHit, 1.0f, -1);
            if (!navMeshHit.hit) {
                SpawnTrash(false);
                aiState = AIState.ThrowTrashGround;
                shouldChangeState = true;
                yield break;
            }
            
            agent.SetDestination(navMeshHit.position);
            var stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.25f);
            yield return new WaitUntil(() => agent.speed < 0.1f || agent.remainingDistance < stoppingDistance);
            shouldChangeState = true;
        }

        private IEnumerator ThrowTrash(bool isAtTrashBin) {
            // wait a bit while stopped
            if (!isAtTrashBin) yield return new WaitForSeconds(Rand.Range(minMaxTrashWaitTime2));
            SpawnTrash(isAtTrashBin);
            trashQueue--;
        }
        
        private bool TrashBinInRadius(out Vector3 trashBinPosition) {
            var bins = Physics.SphereCastAll(transform.position, trashBinSearchDistance, Vector3.left, float.MaxValue).Where(hit => hit.transform.CompareTag("TrashBin")).ToList();

            trashBinPosition = Vector3.zero;
            if (bins.Count <= 0) return false;
            
            trashBinPosition = bins.QuickSorted((hit1, hit2) => hit1.distance.CompareTo(hit2.distance)).First().transform.position;
            return true;
        }

        private IEnumerator DoAfter(float delay, Action action) {
            yield return new WaitForSeconds(delay);
            action();
        }

        /// <summary>
        /// <para>Receives an event from the Event Queue</para>
        /// </summary>
        /// <param name="eventData">Event data raised</param>
        /// <returns><c>true</c> if event propagation should be stopped, <c>false</c> otherwise.</returns>
        public bool OnEvent(EventData eventData) {
            switch (eventData) {
                case EmptyEvent {Type: EventType.NpcThrowTrash}: {
                    if (trashQueue == 0) {
                        // tell the agent to start throwing after a delay trash if none is already queued
                        StartCoroutine(DoAfter(Rand.Range(minMaxTrashWaitTime), () => { shouldChangeState = true; }));
                    }

                    trashQueue++;
                    Debug.Log("I should throw trash, but I wont cause im a good guy lmao");
                    return false;
                }
                default: return false;
            }
        }

        private void SpawnTrash(bool isAtTrashBin) {
            const int maxTries = 10;
            var choice = Rand.ReadOnlyListItem(ResourcesProvider.TrashPickups);

            for (var i = 0; i < maxTries; i++) {
                var position = transform.position;
                var ray = new Ray(new Vector3(position.x, ResourcesProvider.TrashSettings.WorldMaxHeight, position.z), Vector3.down);
                if (Physics.Raycast(ray, out var hit, ResourcesProvider.TrashSettings.WorldMaxHeight + 1000.0f, LayerMask.GetMask("Ground"))) {
                    var spawnY = hit.point.y + ResourcesProvider.TrashSettings.SpawnYOffset;

                    var trash = Instantiate(choice.Prefab, new Vector3(position.x, spawnY, position.z), Quaternion.Euler(0, Rand.Float * 360.0f, 0));
                    trash.Load(choice);
                    trash.transform.localScale = Vector3.zero;
                    
                    if (!isAtTrashBin) trash.transform.DOScale(Vector3.one, ResourcesProvider.TrashSettings.TrashScaleUpDuration);

                    EventQueue.QueueEvent(new TrashEvent(this, EventType.TrashSpawn, trash));
                }
            }
        }

        private enum AIState {
            Wander,
            ThrowTrashGround,
            ApproachBin,
            ThrowTrashBin
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!debug) return;
            if (agent == null) return;
            if (agent.isStopped) {
                Handles.color = new Color(0.98f, 0.15f, 0.13f, 0.65f);
                Handles.DrawSolidDisc(transform.position, Vector3.up, 0.5f);
                return;
            }

            var path = agent.path;
            var lineColor = new Color(0.38f, 0.78f, 0.93f, 0.82f);
            var sphereColor = new Color(0.35f, 0.76f, 0.88f, 0.82f);
            for (var i = 0; i < path.corners.Length - 1; i++) {
                Gizmos.color = lineColor;
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                Gizmos.color = sphereColor;
                Gizmos.DrawSphere(path.corners[i], 0.2f);
            }

            Gizmos.color = new Color(0.35f, 1f, 0.09f, 0.82f);
            Gizmos.DrawSphere(path.corners[path.corners.Length - 1], 0.4f);
        }
#endif
    }
}