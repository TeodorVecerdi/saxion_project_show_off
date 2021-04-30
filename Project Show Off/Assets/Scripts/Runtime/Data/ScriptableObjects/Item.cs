using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
    public class Item : ScriptableObject {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemSprite;
        
        public string ItemName => itemName;
        public Sprite ItemSprite => itemSprite;
    }
}