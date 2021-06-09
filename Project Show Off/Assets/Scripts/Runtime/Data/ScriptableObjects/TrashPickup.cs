using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Trash Pickup", order = 0)]
    public class TrashPickup : ScriptableObject {
        [SerializeField] private string itemName;
        [SerializeField] private TrashMaterial trashCategory;
        [SerializeField] private float minimumMass;
        [SerializeField] private float maximumMass;
        [SerializeField, Tooltip("In seconds")] private float pickupDuration = 1.0f;
        [SerializeField] private float pollutionAmount = 1.0f;
        [SerializeField] private Pickup prefab;

        public string ItemName => itemName;
        public TrashMaterial TrashCategory => trashCategory;
        public float MinimumMass => minimumMass;
        public float MaximumMass => maximumMass;
        public float PickupDuration => pickupDuration;
        public float PollutionAmount => pollutionAmount;
        public Pickup Prefab => prefab;
    }
}