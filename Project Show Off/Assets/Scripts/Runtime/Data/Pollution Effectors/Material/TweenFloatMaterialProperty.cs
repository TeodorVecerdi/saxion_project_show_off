using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewTweenFloatMaterialEffector", menuName = "Data/Pollution Effectors/Tween Material Float", order = 8)]
    public class TweenFloatMaterialProperty : PollutionEffector {
        [SerializeField] private bool specificMaterial = true;
        [SerializeField, ShowIf("nattr__SpecificMaterial")] private Material material;
        [SerializeField, OnValueChanged("UpdatePropertyId")] private string propertyName;
        [SerializeField] private float from;
        [SerializeField] private float to;
        [SerializeField] private float duration = 0.25f;

        [SerializeField, HideInInspector] private int propertyId = -1;

#if UNITY_EDITOR //debug: !! NAUGHTY ATTRIBUTES SPECIFIC 
        // ReSharper disable InconsistentNaming IdentifierTypo UnusedMember.Local=
        private bool nattr__SpecificMaterial => specificMaterial;
        // ReSharper restore InconsistentNaming IdentifierTypo UnusedMember.Local=
#endif

        public override void Apply(Object target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            var mat = material;
            if (!specificMaterial || mat == null) mat = target as Material;
            if (mat == null) return;
           
            if (propertyId == -1) propertyId = Shader.PropertyToID(propertyName);
            material.DOFloat(to, propertyId, duration).From(from);
        }
        
        [UsedImplicitly]
        private void UpdatePropertyId() {
            propertyId = Shader.PropertyToID(propertyName);
        }
    }
}