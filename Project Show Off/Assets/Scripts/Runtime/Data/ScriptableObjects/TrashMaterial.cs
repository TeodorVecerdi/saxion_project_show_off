using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewTrashMaterial", menuName = "Data/Trash Material")]
    public class TrashMaterial : ScriptableObject {
        [SerializeField] private string materialName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite verticalPattern;
        [SerializeField] private Color patternColor;
        [SerializeField] private Types type;
        
        public string MaterialName => materialName;
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