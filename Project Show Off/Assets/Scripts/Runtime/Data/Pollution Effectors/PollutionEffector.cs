using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [Serializable]
    public abstract class PollutionEffector : ScriptableObject {
        public abstract void Apply(Object target, float rawPollution, float pollutionRatio);
    }
    
    public abstract class PollutionEffector<T> : PollutionEffector where T : Object {
        public override void Apply(Object target, float rawPollution, float pollutionRatio) {
            Apply(target as T, rawPollution, pollutionRatio);
        }

        public abstract void Apply(T target, float rawPollution, float pollutionRatio);
    }
}