using UnityEngine;
using UnityEngine.VFX;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetEnabledEffector", menuName = "Data/VFX Pollution/Set Enabled", order = 0)]
    public sealed class SetEnabledEffector : VFXPollutionEffector {
        [SerializeField] private bool enabled;
        public override void Apply(VisualEffect effect, float rawPollution, float pollutionRatio) {
            effect.enabled = enabled;
        }
    }
}