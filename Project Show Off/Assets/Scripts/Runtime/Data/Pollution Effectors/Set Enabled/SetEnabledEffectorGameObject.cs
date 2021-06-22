using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetEnabledEffector", menuName = "Data/Pollution Effectors/Set Enabled/GameObject", order = 1)]
    public sealed class SetEnabledEffectorGameObject : PollutionEffector<GameObject> {
        [SerializeField] private bool enabled;
     
        public override void Apply(GameObject target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            target.SetActive(enabled);
        }
    }
}