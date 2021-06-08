using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewBuildableObject", menuName = "Data/Buildable Object", order = 0)]
    public class BuildableObject : ScriptableObject {
        [SerializeField] private string objectName;
        [SerializeField, Multiline] private string objectDescription;
        [SerializeField] private Sprite objectSprite;
        [SerializeField] private BuildableObjectPreview prefab;
        [SerializeField] private MaterialInventory constructionRequirements;
        [SerializeField, Tooltip("Any location where the player can place this object")] private List<BuildArea> buildAreas;

        public Sprite ObjectSprite => objectSprite;
        public BuildableObjectPreview Prefab => prefab;
        public MaterialInventory ConstructionRequirements => constructionRequirements;
        public List<BuildArea> BuildAreas => buildAreas;
        public string Name => objectName;
        public string Description => objectDescription;
    }
}