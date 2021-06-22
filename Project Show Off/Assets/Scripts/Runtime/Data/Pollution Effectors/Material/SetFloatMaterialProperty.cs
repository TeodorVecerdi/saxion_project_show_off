using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetFloatMaterialValueEffector", menuName = "Data/Pollution Effectors/Set Material Float", order = 8)]
    public class SetFloatMaterialProperty : PollutionEffector {
        [SerializeField] private bool specificMaterial = true;
        [SerializeField, ShowIf("nattr__SpecificMaterial")] private Material material;
        [SerializeField, OnValueChanged("UpdatePropertyId")] private string propertyName;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

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
            mat.SetFloat(propertyId, curve.Evaluate(pollutionRatio));
        }

        [UsedImplicitly]
        private void UpdatePropertyId() {
            propertyId = Shader.PropertyToID(propertyName);
        }
    }
}