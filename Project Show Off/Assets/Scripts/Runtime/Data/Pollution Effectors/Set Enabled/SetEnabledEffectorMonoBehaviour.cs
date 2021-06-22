using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetEnabledEffector", menuName = "Data/Pollution Effectors/Set Enabled/MonoBehaviour", order = 2)]
    public sealed class SetEnabledEffectorMonoBehaviour : PollutionEffector<MonoBehaviour> {
        [SerializeField] private bool enabled;
        public override void Apply(MonoBehaviour target, float rawPollution, float pollutionRatio) {
            target.enabled = enabled;
        }
    }
}