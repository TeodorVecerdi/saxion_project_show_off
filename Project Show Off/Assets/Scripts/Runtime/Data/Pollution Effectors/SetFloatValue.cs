using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSetFloatValueEffector", menuName = "Data/Pollution Effectors/Set Float", order = 8)]
    public class SetFloatValue : PollutionEffector {
        [SerializeField] private UnityEvent<float> setValueFunction;
        
        public override void Apply(Object target, float rawPollution, float pollutionRatio) {
            throw new System.NotImplementedException();
        }
    }
}