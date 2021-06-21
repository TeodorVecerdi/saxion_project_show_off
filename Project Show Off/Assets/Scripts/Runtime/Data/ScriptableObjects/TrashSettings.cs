using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "TrashSettings", menuName = "Data/TrashSettings", order = 0)]
    public class TrashSettings : ScriptableObject {
        [SerializeField] private float trashScaleUpDuration = 0.1f;
        [SerializeField] private float spawnYOffset = 0.25f;
        [SerializeField] private float worldMaxHeight = 100.0f;
        [SerializeField] private float baseSpawnInterval = 10.0f;

        public float TrashScaleUpDuration => trashScaleUpDuration;
        public float SpawnYOffset => spawnYOffset;
        public float WorldMaxHeight => worldMaxHeight;
        public float BaseSpawnInterval => baseSpawnInterval;
    }
}