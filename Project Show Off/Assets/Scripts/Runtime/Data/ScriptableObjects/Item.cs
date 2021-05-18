using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item", order = 0)]
    public class Item : ScriptableObject {
        [SerializeField] private string itemName;
        [SerializeField] private TrashCategory trashCategory;
        [SerializeField] private float minimumMass;
        [SerializeField] private float maximumMass;

        public string ItemName => itemName;
        public TrashCategory TrashCategory => trashCategory;
        public float MinimumMass => minimumMass;
        public float MaximumMass => maximumMass;
    }
}