using Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor {
    [CustomEditor(typeof(TrashBin))]
    public class TrashBinEditor : UnityEditor.Editor {
        private static bool enableEditMode;
        private static Color32 handleColor = new Color32(0, 197, 108, 180);
        private TrashBin trashBin;

        private void OnEnable() {
            LoadSettings();
            trashBin = target as TrashBin;
        }

        private void OnDisable() {
            SaveSettings();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EditorGUI.BeginChangeCheck();
            var newEnableEditMode = EditorGUILayout.Toggle("Enable Edit Mode", enableEditMode);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this, "Change trash bin edit mode state");
                enableEditMode = newEnableEditMode;
            }
            
            if (enableEditMode) {
                EditorGUI.BeginChangeCheck();
                var newHandleColor = EditorGUILayout.ColorField("Editor Color", handleColor);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "Change handles color");
                    handleColor = newHandleColor;
                }
            }
        }
        
        private void OnSceneGUI() {
            if(!enableEditMode) return;
            
            var zTest = Handles.zTest;
            Handles.zTest = CompareFunction.LessEqual;
            Handles.color = handleColor;
            
            Handles.DrawSolidDisc(trashBin.transform.position, Vector3.up, trashBin.CollectionRadius);
            
            EditorGUI.BeginChangeCheck();
            Handles.color = Color.white;
            var newRadius = Handles.RadiusHandle(Quaternion.identity, trashBin.transform.position, trashBin.CollectionRadius);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(trashBin, "Change trash bin radius");
                trashBin.CollectionRadius = newRadius;
            }
            

            Handles.zTest = zTest;
        }

        private void LoadSettings() {
            if (!EditorPrefs.HasKey("TRASH_BIN_EDIT_MODE")) EditorPrefs.SetBool("TRASH_BIN_EDIT_MODE", enableEditMode);
            else enableEditMode = EditorPrefs.GetBool("TRASH_BIN_EDIT_MODE");
            if (!EditorPrefs.HasKey("TRASH_BIN_HANDLES_COLOR")) EditorPrefs.SetInt("TRASH_BIN_HANDLES_COLOR", Utilities.PackColor(handleColor));
            else handleColor = Utilities.UnpackColor(EditorPrefs.GetInt("TRASH_BIN_HANDLES_COLOR"));
        }

        private void SaveSettings() {
            EditorPrefs.SetBool("TRASH_BIN_EDIT_MODE", enableEditMode);
            EditorPrefs.SetInt("TRASH_BIN_HANDLES_COLOR", Utilities.PackColor(handleColor));

        }
    }
}