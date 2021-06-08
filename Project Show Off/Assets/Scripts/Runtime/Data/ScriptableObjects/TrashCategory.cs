using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewTrashCategory", menuName = "Data/Trash Category")]
    public class TrashCategory : ScriptableObject {
        [SerializeField] private string categoryName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite verticalPattern;
        [SerializeField] private Color patternColor;
        [SerializeField] private Types type;
        
        public string CategoryName => categoryName;
        public Sprite Icon => icon;
        public Sprite VerticalPattern => verticalPattern;
        public Color PatternColor => patternColor;
        public Types Type => type;


        public enum Types {
            Glass,
            Metal,
            Paper,
            Plastic
        }
    }
}