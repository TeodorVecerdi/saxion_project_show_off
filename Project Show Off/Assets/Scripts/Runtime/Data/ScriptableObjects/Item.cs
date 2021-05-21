using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item", order = 0)]
    public class Item : ScriptableObject {
        [SerializeField] private string itemName;
        [SerializeField] private TrashCategory trashCategory;
        [SerializeField] private float minimumMass;
        [SerializeField] private float maximumMass;
        [SerializeField, Tooltip("In seconds")] private float pickupDuration = 1.0f;

        public string ItemName => itemName;
        public TrashCategory TrashCategory => trashCategory;
        public float MinimumMass => minimumMass;
        public float MaximumMass => maximumMass;
        public float PickupDuration => pickupDuration;
    }
}