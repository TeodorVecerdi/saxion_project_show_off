using Runtime;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(CameraBoundaries))]
    public class CameraBoundariesHandles : UnityEditor.Editor {
        private static readonly Color rectangleColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
        [SerializeField] private CameraBoundaries boundaries;
        [SerializeField] private Vector3[] vertices;
        [SerializeField] private bool enableEditMode;

        private void OnEnable() {
            boundaries = (CameraBoundaries) target;
            vertices = new Vector3[4];
            RecalculateRectVertices();
        }

        public override void OnInspectorGUI() {
            enableEditMode = EditorGUILayout.Toggle("Enable Edit Mode", enableEditMode);
        }

        private void OnSceneGUI() {
            if(!enableEditMode) return;
            
            EditorGUI.BeginChangeCheck();
            var newMinimumPosition = Handles.PositionHandle(boundaries.MinimumPosition, Quaternion.identity);
            newMinimumPosition.y = 0;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(boundaries, "Change minimum boundary");
                boundaries.MinimumPosition = newMinimumPosition;
                RecalculateRectVertices();
            }
            
            EditorGUI.BeginChangeCheck();
            var newMaximumPosition = Handles.PositionHandle(boundaries.MaximumPosition, Quaternion.identity);
            newMaximumPosition.y = 0;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(boundaries, "Change maximum boundary");
                boundaries.MaximumPosition = newMaximumPosition;
                RecalculateRectVertices();
            }
            
            Handles.DrawSolidRectangleWithOutline(vertices, rectangleColor, Color.clear);
        }

        private void RecalculateRectVertices() {
            vertices = new [] {
                boundaries.MinimumPosition,
                new Vector3(boundaries.MinimumPosition.x, 0, boundaries.MaximumPosition.z),
                boundaries.MaximumPosition,
                new Vector3(boundaries.MaximumPosition.x, 0, boundaries.MinimumPosition.z)
            };
        }
    }
}