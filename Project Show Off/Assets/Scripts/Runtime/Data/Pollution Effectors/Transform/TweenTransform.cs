using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "NewTweenTransformEffector", menuName = "Data/Pollution Effectors/Tween Transform", order = 8)]
    public class TweenTransform : PollutionEffector {
        [SerializeField] private bool specificTarget = true;
        [SerializeField, ShowIf("nattr__SpecificTarget")] private Transform transform;
        [SerializeField] private Vector3 from;
        [SerializeField] private Vector3 to;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private Operation operation;

#if UNITY_EDITOR //debug: !! NAUGHTY ATTRIBUTES SPECIFIC 
        // ReSharper disable InconsistentNaming IdentifierTypo UnusedMember.Local=
        private bool nattr__SpecificTarget => specificTarget;
        // ReSharper restore InconsistentNaming IdentifierTypo UnusedMember.Local=
#endif

        public override void Apply(Object target, Dictionary<string, object> extraData, float rawPollution, float pollutionRatio) {
            var targetTransform = transform;
            if (!specificTarget || targetTransform == null) targetTransform = target as Transform;
            if (targetTransform == null) return;

            switch (operation) {
                case Operation.Scale:
                    targetTransform.DOScale(to, duration);
                    break;
                case Operation.RotationLocal:
                    targetTransform.DOLocalRotate(to, duration);
                    break;

                case Operation.RotationWorld:
                    targetTransform.DORotate(to, duration);
                    break;

                case Operation.PositionLocal:
                    targetTransform.DOLocalMove(to, duration);
                    break;

                case Operation.PositionWorld:
                    targetTransform.DOMove(to, duration);
                    break;
            }
        }

        private enum Operation {
            Scale,
            RotationLocal,
            RotationWorld,
            PositionLocal,
            PositionWorld
        }
    }
}