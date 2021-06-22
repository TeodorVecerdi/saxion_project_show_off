using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetEnabledEffector", menuName = "Data/Pollution Effectors/Set Enabled/Visual Effect", order = 3)]
    public sealed class SetEnabledEffectorVFX : PollutionEffector<VisualEffect> {
        [SerializeField] private bool enabled;

        public override void Apply(VisualEffect target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            target.enabled = enabled;
        }
    }
}