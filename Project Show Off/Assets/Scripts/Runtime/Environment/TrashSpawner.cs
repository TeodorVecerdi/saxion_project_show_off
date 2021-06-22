using DG.Tweening;
using NaughtyAttributes;
using Runtime.Event;
using UnityCommons;
using UnityEditor;
using UnityEngine;
using EventType = Runtime.Event.EventType;

namespace Runtime {
    public sealed class TrashSpawner : MonoBehaviour {
        [Header("Location")]
        [SerializeField] private Vector3 from;
        [SerializeField] private Vector3 to;
        [SerializeField] private float worldMaxHeight;
        [Header("Settings")]
        [SerializeField] private float spawnInterval = 4f;
        [SerializeField] private float trashScaleUpDuration = 0.1f;
        [SerializeField, MinValue(0)] private int initialTrashCount = 20;
        [SerializeField] private float yOffset = 0.25f;

        private float spawnTimer;

        private void Start() {
            SpawnInitialTrash();
        }

        private void Update() {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval) {
                spawnTimer -= spawnInterval;
                #if !ENABLE_NPCS
                    SpawnTrash();
                #else
                    EventQueue.QueueEvent(new EmptyEvent(this, EventType.NpcThrowTrash));
                #endif
            }
        }

        private void SpawnInitialTrash() {
            var totalPollution = 0.0f;
            for (var i = 0; i < initialTrashCount; i++) {
                totalPollution += SpawnTrash(false);
            }

            EventQueue.QueueEvent(new PollutionChangeEvent(this, totalPollution));
        }

        private float SpawnTrash(bool sendEvent = true) {
            const int maxTries = 10;
            var choice = Rand.ReadOnlyListItem(ResourcesProvider.TrashPickups);

            for (var i = 0; i < maxTries; i++) {
                var spawnX = Rand.Range(from.x, to.x);
                var spawnZ = Rand.Range(from.z, to.z);
                var ray = new Ray(new Vector3(spawnX, worldMaxHeight + 25.0f, spawnZ), Vector3.down);
                if (Physics.Raycast(ray, out var hit, worldMaxHeight + 1000f, LayerMask.GetMask("Ground"))) {
                    var spawnY = hit.point.y + yOffset;

                    var trash = Instantiate(choice.Prefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.Euler(0, Rand.Float * 360.0f, 0), transform);
                    trash.Load(choice);
                    trash.transform.localScale = Vector3.zero;
                    trash.transform.DOScale(Vector3.one, trashScaleUpDuration);
                    if (sendEvent)
                        EventQueue.QueueEvent(new TrashEvent(this, EventType.TrashSpawn, trash));
                    return choice.PollutionAmount;
                }
            }

            return 0.0f;
        }

        private void OnDrawGizmosSelected() {
#if UNITY_EDITOR
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(from, HandleUtility.GetHandleSize(from) * 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(to, HandleUtility.GetHandleSize(to) * 0.2f);

            var diff = to - from;
            var avg = 0.5f * (from + to);
            var halfHeight = worldMaxHeight * 0.5f;
            var center = new Vector3(avg.x, halfHeight, avg.z);
            var size = new Vector3(Mathf.Abs(diff.x), worldMaxHeight, Mathf.Abs(diff.z));
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);
            Gizmos.DrawCube(center, size);
#endif
        }
    }
}