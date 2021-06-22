using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [Serializable]
    public abstract class PollutionEffector : ScriptableObject {

        public abstract void Apply(Object target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio);
    }
    
    public abstract class PollutionEffector<T> : PollutionEffector where T : Object {
        public override void Apply(Object target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            Apply(target as T, extraData, rawPollution, pollutionRatio);
        }

        public abstract void Apply(T target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio);
    }
}