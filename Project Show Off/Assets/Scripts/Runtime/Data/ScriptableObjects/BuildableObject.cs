using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewBuildableObject", menuName = "Data/Buildable Object", order = 0)]
    public sealed class BuildableObject : ScriptableObject {
        [SerializeField] private string objectName;
        [SerializeField, Multiline] private string objectDescription;
        [SerializeField] private Sprite objectSprite;
        [SerializeField] private BuildableObjectPreview prefab;
        [SerializeField] private MaterialInventory constructionRequirements;
        [SerializeField, Tooltip("Any location where the player can place this object")] private List<BuildArea> buildAreas;
        [Space]
        [SerializeField] private int buildScore = 10;
        [Range(0.0f, 1.0f), SerializeField] private float peopleHappinessAmount = 0.5f;
        [Range(0.0f, 1.0f), SerializeField] private float biodiversityHappinessAmount = 0.0f;

        public Sprite ObjectSprite => objectSprite;
        public BuildableObjectPreview Prefab => prefab;
        public MaterialInventory ConstructionRequirements => constructionRequirements;
        public List<BuildArea> BuildAreas => buildAreas;
        public string Name => objectName;
        public string Description => objectDescription;
        public int BuildScore => buildScore;
        public float PeopleHappinessAmount => peopleHappinessAmount;
        public float BiodiversityHappinessAmount => biodiversityHappinessAmount;
    }
}