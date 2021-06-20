using NaughtyAttributes;
using UnityCommons;
using UnityEngine;
using UnityEngine.VFX;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewSpawnRateEffector", menuName = "Data/VFX Pollution/Change Spawn Rate", order = 0)]
    public sealed class ChangeSpawnRateEffector : VFXPollutionEffector {
        [InfoBox("Sets the spawn rate of a VFX Graph according to the current pollution percentage based on the Curve.\n'Max Value' means the maximum value that the VFX Graph supports for Spawn Rate, which for Bees and Flies is 50")]
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private bool invertCurve;
        [SerializeField] private float maxValue = 50.0f;
        
        private static readonly int spawnRateId = Shader.PropertyToID("Spawn Rate");
        
        public override void Apply(VisualEffect effect, float rawPollution, float pollutionRatio) {
            var time = pollutionRatio.Clamped01();
            if (invertCurve) time = 1 - time;
            var spawnRate = (curve.Evaluate(time) * maxValue).Clamped(0.0f, maxValue);
            
            effect.SetFloat(spawnRateId, spawnRate);
        }
    }
}