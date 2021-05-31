using System.Collections.Generic;
using Editor.Utils;
using Runtime;
using Runtime.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor {
    [CustomEditor(typeof(BuildArea))]
    public class BuildAreaEditor : UnityEditor.Editor {
        private static Material sPreviewMaterial;
        
        [SerializeField] private BuildArea buildArea;
        [SerializeField] private int hoveredQuad = -1;
        [SerializeField] private int selectedQuad = -1;
        [SerializeField] private bool moveWholeQuad;
        [SerializeField] private float bakedMeshTolerance = 0.1f;
        
        [SerializeField] private bool isMeshPreviewActive;
        [SerializeField] private GameObject previewGameObject;

        private void OnEnable() {
            buildArea = target as BuildArea;
            SceneView.duringSceneGui += DrawSceneGUI;
            if (sPreviewMaterial == null) 
                sPreviewMaterial = Resources.Load<Material>("_Materials/BuildAreaPreview");
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= DrawSceneGUI;
            AssetDatabase.SaveAssets();

            if (previewGameObject != null) {
                DestroyImmediate(previewGameObject);
                previewGameObject = null;
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            bakedMeshTolerance = EditorGUILayout.FloatField("Mesh thickness", bakedMeshTolerance);

            if (buildArea.IsBakeDirty) {
                EditorGUILayout.HelpBox("Build area has changed since last bake. Please bake again if you want to keep the changes", MessageType.Error);
            }
            if (GUILayout.Button("Bake")) {
                BakeQuads();
                buildArea.IsBakeDirty = false;
            }

            GUI.enabled = buildArea.BakedMesh != null;
            EditorGUI.BeginChangeCheck();
            var newIsMeshPreviewActive = EditorGUILayout.Toggle("Preview Baked Mesh", isMeshPreviewActive);
            if (EditorGUI.EndChangeCheck()) {
                if (!newIsMeshPreviewActive && isMeshPreviewActive) {
                    DestroyImmediate(previewGameObject);
                    previewGameObject = null;
                } else if (newIsMeshPreviewActive && !isMeshPreviewActive) {
                    previewGameObject = new GameObject("PREVIEW BUILD AREA MESH");
                    var meshFilter = previewGameObject.AddComponent<MeshFilter>();
                    var meshRenderer = previewGameObject.AddComponent<MeshRenderer>();
                    meshFilter.sharedMesh = buildArea.BakedMesh;
                    meshRenderer.sharedMaterial = sPreviewMaterial;
                }

                isMeshPreviewActive = newIsMeshPreviewActive;
            }
            GUI.enabled = true;
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
                        EditorUtilities.RecordChange(buildArea, "Move quad", () => {
                            buildArea.IsBakeDirty = true;
                            quad.Points[0] += difference;
                            quad.Points[1] += difference;
                            quad.Points[2] += difference;
                            quad.Points[3] += difference;
                        });
                    }
                } else {
                    EditorGUI.BeginChangeCheck();
                    var newVert0 = Handles.DoPositionHandle(quad.Points[0], Quaternion.identity);
                    var newVert1 = Handles.DoPositionHandle(quad.Points[1], Quaternion.identity);
                    var newVert2 = Handles.DoPositionHandle(quad.Points[2], Quaternion.identity);
                    var newVert3 = Handles.DoPositionHandle(quad.Points[3], Quaternion.identity);
                    if (EditorGUI.EndChangeCheck()) {
                        EditorUtilities.RecordChange(buildArea, "Move quad vertex", () => {
                            buildArea.IsBakeDirty = true;
                            quad.Points[0] = newVert0;
                            quad.Points[1] = newVert1;
                            quad.Points[2] = newVert2;
                            quad.Points[3] = newVert3;    
                        });
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
                        EditorUtilities.RecordChange(buildArea, "Added quad", () => {
                            buildArea.IsBakeDirty = true;
                            buildArea.AddQuad(hitInfo.point);
                        });
                    }

                    break;
                }
                case {type: EventType.KeyUp} keyUpEvent: {
                    if (selectedQuad == -1) break;
                    switch (keyUpEvent) {
                        case {keyCode: KeyCode.Backspace}: {
                            EditorUtilities.RecordChange(buildArea, "Removed quad", () => {
                                buildArea.IsBakeDirty = true;
                                buildArea.RemoveQuad(selectedQuad);
                            });
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

        private void BakeQuads() {
            Undo.RecordObject(buildArea, "Baked build area mesh");
            
            // Setup for first time baking
            if (buildArea.BakedMesh == null) {
                buildArea.BakedMesh = new Mesh {name = "Build Area Mesh"};
                AssetDatabase.AddObjectToAsset(buildArea.BakedMesh, buildArea);
            }

            BuildAreaBaker.Bake(buildArea.Quads, bakedMeshTolerance, buildArea.BakedMesh);
            EditorUtility.SetDirty(buildArea);
            AssetDatabase.SaveAssets();
        }
    }
}