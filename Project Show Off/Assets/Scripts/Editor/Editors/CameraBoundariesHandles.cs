using Runtime;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(CameraBoundaries))]
    public class CameraBoundariesHandles : UnityEditor.Editor {
        [SerializeField] private CameraBoundaries boundaries;
        [SerializeField] private Vector3[] vertices;
        [SerializeField] private bool enableEditMode;
        [SerializeField] private Color rectangleColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
        [SerializeField] private float rectangleHeight;

        private void OnEnable() {
            boundaries = (CameraBoundaries) target;
            vertices = new Vector3[4];
            RecalculateRectVertices();
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            var newEnableEditMode = EditorGUILayout.Toggle("Enable Edit Mode", enableEditMode);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this, "Change boundaries edit mode state");
                enableEditMode = newEnableEditMode;
            }

            if (enableEditMode) {
                EditorGUI.BeginChangeCheck();
                var newRectangleColor = EditorGUILayout.ColorField("Boundaries Color", rectangleColor);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "Change boundaries rectangle color");
                    rectangleColor = newRectangleColor;
                }
                
                EditorGUI.BeginChangeCheck();
                var newRectangleHeight = EditorGUILayout.FloatField("Boundaries Height (Visual Only)", rectangleHeight);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "Change boundaries rectangle height");
                    rectangleHeight = newRectangleHeight;
                    RecalculateRectVertices();
                }
            }
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
                boundaries.MinimumPosition + Vector3.up * rectangleHeight,
                new Vector3(boundaries.MinimumPosition.x, 0, boundaries.MaximumPosition.z) + Vector3.up * rectangleHeight,
                boundaries.MaximumPosition + Vector3.up * rectangleHeight,
                new Vector3(boundaries.MaximumPosition.x, 0, boundaries.MinimumPosition.z) + Vector3.up * rectangleHeight
            };
        }
    }
}