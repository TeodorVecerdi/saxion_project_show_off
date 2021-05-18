using UnityEngine;

namespace Runtime {
    public class CameraBoundaries : MonoBehaviour {
        public Vector3 MinimumPosition {
            get => minimumPosition;
            set => minimumPosition = value;
        }
        public Vector3 MaximumPosition {
            get => maximumPosition;
            set => maximumPosition = value;
        }

        [SerializeField] private Vector3 minimumPosition = Vector3.zero;
        [SerializeField] private Vector3 maximumPosition = Vector3.zero;

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(minimumPosition, 0.5f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(maximumPosition, 0.5f);
        }
    }
}