using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Runtime.Data {
    public abstract class VFXPollutionEffector : ScriptableObject {
        public abstract void Apply(VisualEffect effect, float rawPollution, float pollutionRatio);
    }
}