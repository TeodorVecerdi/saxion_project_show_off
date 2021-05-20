using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewBuildableObject", menuName = "Data/BuildableObjects", order = 0)]
    public class BuildableObject : ScriptableObject {
        [SerializeField] private Sprite objectSprite;
        [SerializeField] private BuildableObjectPreview prefab;

        public Sprite ObjectSprite => objectSprite;
        public BuildableObjectPreview Prefab => prefab;
    }
}