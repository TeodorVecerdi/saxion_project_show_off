using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Editor.Utils {
    public static class EditorUtilities {
        public static void RecordChange(Object target, string undoMessage, Action applyChangesAction) {
            Undo.RecordObject(target, undoMessage);
            applyChangesAction?.Invoke();
            EditorUtility.SetDirty(target);
        }
    }
}