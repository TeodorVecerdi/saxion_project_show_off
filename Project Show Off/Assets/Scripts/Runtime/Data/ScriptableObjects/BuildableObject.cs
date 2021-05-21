using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewBuildableObject", menuName = "Data/Buildable Object", order = 0)]
    public class BuildableObject : ScriptableObject {
        [SerializeField] private Sprite objectSprite;
        [SerializeField] private BuildableObjectPreview prefab;
        [SerializeField] private MaterialInventory constructionRequirements;

        public Sprite ObjectSprite => objectSprite;
        public BuildableObjectPreview Prefab => prefab;
        public MaterialInventory ConstructionRequirements => constructionRequirements;
    }
}