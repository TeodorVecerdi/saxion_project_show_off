using System;
using Runtime;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(CameraBoundaries))]
    public class CameraBoundariesHandles : UnityEditor.Editor {
        [SerializeField] private CameraBoundaries boundaries;
        [SerializeField] private Vector3[] vertices;
        private static bool enableEditMode;
        private static Color32 rectangleColor = new Color32(255, 255, 255, 64);

        private void OnEnable() {
            boundaries = (CameraBoundaries) target;
            vertices = new Vector3[4];
            LoadSettings();
            RecalculateRectVertices();
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDisable() {
            SaveSettings();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed() {
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

        private void LoadSettings() {
            if (!EditorPrefs.HasKey("CAMERA_BOUNDARIES_EDIT_MODE")) EditorPrefs.SetBool("CAMERA_BOUNDARIES_EDIT_MODE", enableEditMode);
            else enableEditMode = EditorPrefs.GetBool("CAMERA_BOUNDARIES_EDIT_MODE");
            if (!EditorPrefs.HasKey("CAMERA_BOUNDARIES_RECT_COLOR")) EditorPrefs.SetInt("CAMERA_BOUNDARIES_RECT_COLOR", Utilities.PackColor(rectangleColor));
            else rectangleColor = Utilities.UnpackColor(EditorPrefs.GetInt("CAMERA_BOUNDARIES_RECT_COLOR"));
        }

        private void SaveSettings() {
            EditorPrefs.SetBool("CAMERA_BOUNDARIES_EDIT_MODE", enableEditMode);
            EditorPrefs.SetInt("CAMERA_BOUNDARIES_RECT_COLOR", Utilities.PackColor(rectangleColor));
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