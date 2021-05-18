using UnityEngine;

namespace Runtime {
    public class BuildModeCameraBoundaries : MonoBehaviour {
        public Vector2 MinimumPosition {
            get => minimumPosition;
            set => minimumPosition = value;
        }
        public Vector2 MaximumPosition {
            get => maximumPosition;
            set => maximumPosition = value;
        }

        [SerializeField] private Vector2 minimumPosition = Vector2.zero;
        [SerializeField] private Vector2 maximumPosition = Vector2.one;

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(minimumPosition.x, 0, minimumPosition.y), 0.5f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(maximumPosition.x, 0, maximumPosition.y), 0.5f);
        }
    }
}