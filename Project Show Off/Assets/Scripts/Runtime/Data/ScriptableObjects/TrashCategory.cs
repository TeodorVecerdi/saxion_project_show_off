using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewTrashCategory", menuName = "Data/Trash Category")]
    public class TrashCategory : ScriptableObject {
        [SerializeField] private string categoryName;
        [SerializeField] private Sprite categorySprite;
        
        public string CategoryName => categoryName;
        public Sprite CategorySprite => categorySprite;
    }
}