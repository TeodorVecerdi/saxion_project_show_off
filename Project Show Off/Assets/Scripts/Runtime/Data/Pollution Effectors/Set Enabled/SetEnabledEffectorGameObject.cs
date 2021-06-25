using System.Collections.Generic;
using NaughtyAttributes;
using UnityCommons;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetEnabledEffector", menuName = "Data/Pollution Effectors/Set Enabled/GameObject", order = 1)]
    public sealed class SetEnabledEffectorGameObject : PollutionEffector<GameObject> {
        [SerializeField] private bool enabled;
        [SerializeField] private bool withDelay;
        [SerializeField, ShowIf("nattr__WithDelay")] private float delay = 0.0f;
        
#if UNITY_EDITOR //debug: !! NAUGHTY ATTRIBUTES SPECIFIC 
        // ReSharper disable InconsistentNaming IdentifierTypo UnusedMember.Local=
        private bool nattr__WithDelay => withDelay;
        // ReSharper restore InconsistentNaming IdentifierTypo UnusedMember.Local=
#endif
     
        public override void Apply(GameObject target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            if (!withDelay) {
                target.SetActive(enabled);
                return;
            }

            Run.After(delay, () => {
                target.SetActive(enabled);
            });
        }
    }
}