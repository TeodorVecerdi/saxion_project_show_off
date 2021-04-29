using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
    public class Item : ScriptableObject {
        [FormerlySerializedAs("Name")] [SerializeField] private string itemName;
        public string ItemName => itemName;
    }
}