using System;
using NaughtyAttributes;
using UnityCommons;
using UnityEngine;
using UnityEngine.VFX;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewConditionalEffector", menuName = "Data/VFX Pollution/Conditional", order = 0)]
    public class ConditionalEffector : VFXPollutionEffector {
        [InfoBox("Applies 'If True' is the pollution percentage matches the condition and value, and applies 'If False' otherwise.\n\nYou can use this, for example, to activate a certain VFX if the pollution is over a certain value")]
        [SerializeField, Range(0.0f, 1.0f)] private float value = 0.5f;
        [SerializeField] private Condition condition;
        [SerializeField] private VFXPollutionEffector ifTrue;
        [SerializeField] private VFXPollutionEffector ifFalse;
        
        public override void Apply(VisualEffect effect, float rawPollution, float pollutionRatio) {
            var result = condition switch {
                Condition.Less => pollutionRatio < value,
                Condition.LessOrEqual => pollutionRatio <= value,
                Condition.Greater => pollutionRatio > value,
                Condition.GreaterOrEqual => pollutionRatio >= value,
                Condition.Equal => Math.Abs(pollutionRatio - value) < 0.001f,
                Condition.NotEqual => Math.Abs(pollutionRatio - value) > 0.001f,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (result) {
                if (ifTrue != null) ifTrue.Apply(effect, rawPollution, pollutionRatio);
            } else {
                if(ifFalse != null) ifFalse.Apply(effect, rawPollution, pollutionRatio);
            }
        }

        [Serializable]
        private enum Condition {
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual,
            Equal,
            NotEqual
        }
    }
}