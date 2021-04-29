using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
    public class ItemSO : ScriptableObject {
        [SerializeField] private string Name;
    }
}