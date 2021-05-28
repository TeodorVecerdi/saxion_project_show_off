using System;
using System.Collections.Generic;
using Runtime;
using Runtime.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Editor {
    [CustomEditor(typeof(BuildArea))]
    public class BuildAreaEditor : UnityEditor.Editor {
        [SerializeField] private BuildArea buildArea;
        [SerializeField] private int hoveredQuad = -1;
        [SerializeField] private int selectedQuad = -1;
        [SerializeField] private bool moveWholeQuad;

        private void OnEnable() {
            buildArea = target as BuildArea;
            SceneView.duringSceneGui += DrawSceneGUI;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= DrawSceneGUI;
        }

        private void DrawSceneGUI(SceneView sceneView) {
            var zTest = Handles.zTest;
            HandleInput();
            Handles.zTest = CompareFunction.LessEqual;

            for (var i = 0; i < buildArea.Quads.Count; i++) {
                var quad = buildArea.Quads[i];
                var quadColor = Color.gray;
                if (hoveredQuad == i) quadColor = Color.yellow;
                if (selectedQuad == i) quadColor = Color.green;
                DrawQuad(quad, quadColor);
            }

            if (selectedQuad != -1) {
                var quad = buildArea.Quads[selectedQuad];
                if (moveWholeQuad) {
                    EditorGUI.BeginChangeCheck();
                    var center = quad.Points[0] + quad.Points[1] + quad.Points[2] + quad.Points[3];
                    center *= 0.25f;
                    var newCenter = Handles.DoPositionHandle(center, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck()) {
                        var difference = newCenter - center;
                        Undo.RecordObject(buildArea, "Move quad");
                        quad.Points[0] += difference;
                        quad.Points[1] += difference;
                        quad.Points[2] += difference;
                        quad.Points[3] += difference;
                    }
                } else {
                    EditorGUI.BeginChangeCheck();
                    var newVert0 = Handles.DoPositionHandle(quad.Points[0], Quaternion.identity);
                    var newVert1 = Handles.DoPositionHandle(quad.Points[1], Quaternion.identity);
                    var newVert2 = Handles.DoPositionHandle(quad.Points[2], Quaternion.identity);
                    var newVert3 = Handles.DoPositionHandle(quad.Points[3], Quaternion.identity);
                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(buildArea, "Move quad vertex");
                        quad.Points[0] = newVert0;
                        quad.Points[1] = newVert1;
                        quad.Points[2] = newVert2;
                        quad.Points[3] = newVert3;
                    }
                }
            }

            Handles.zTest = zTest;
        }

        private void DrawQuad(BuildArea.Quad quad, Color color) {
            var handleColor = Handles.color;

            Handles.color = color;
            Handles.DrawAAConvexPolygon(quad.Points);

            Handles.color = Color.black;
            Handles.DrawAAPolyLine(quad.Points);

            Handles.color = handleColor;
        }

        private void HandleInput() {
            var currentEvent = Event.current;
            var controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            switch (currentEvent) {
                case {type: EventType.MouseMove}: {
                    // Update hovered quad
                    var mousePosition = currentEvent.mousePosition;
                    hoveredQuad = -1;
                    for (var index = buildArea.Quads.Count - 1; index >= 0; index--) {
                        var quadPoints = buildArea.Quads[index].Points;

                        if (Utilities.IsMouseInQuad(mousePosition,
                            HandleUtility.WorldToGUIPoint(quadPoints[0]), HandleUtility.WorldToGUIPoint(quadPoints[1]),
                            HandleUtility.WorldToGUIPoint(quadPoints[2]), HandleUtility.WorldToGUIPoint(quadPoints[3]))
                        ) {
                            hoveredQuad = index;
                            break;
                        }
                    }

                    break;
                }
                case {type: EventType.MouseDown, button: 0, alt: false, control: false}: {
                    if (HandleUtility.nearestControl != controlID) break;
                    GUIUtility.hotControl = controlID;
                    currentEvent.Use();
                    selectedQuad = hoveredQuad;
                    break;
                }
                case {type: EventType.MouseDown, button: 0, alt: false, control: true}: {
                    if (HandleUtility.nearestControl != controlID) break;
                    // create new quad at mouse position
                    var mousePosition = currentEvent.mousePosition;
                    if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(mousePosition), out var hitInfo)) {
                        Undo.RecordObject(buildArea, "Added quad");
                        buildArea.AddQuad(hitInfo.point);
                    }

                    break;
                }
                case {type: EventType.KeyUp} keyUpEvent: {
                    if (selectedQuad == -1) break;
                    switch (keyUpEvent) {
                        case {keyCode: KeyCode.Backspace}: {
                            Undo.RecordObject(buildArea, "Removed quad");
                            buildArea.RemoveQuad(selectedQuad);
                            selectedQuad = -1;
                            break;
                        }
                        case {keyCode: KeyCode.A}: {
                            moveWholeQuad = !moveWholeQuad;
                            break;
                        }
                    }

                    break;
                }
                case {type: EventType.Layout}: {
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                }
            }
        }
    }
}