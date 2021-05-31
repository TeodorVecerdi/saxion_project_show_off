using System;
using System.Collections.Generic;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "New Build Area", menuName = "Data/Build Area", order = 0)]
    public class BuildArea : ScriptableObject {
        [SerializeField] private List<Quad> quads;
        [SerializeField, ReadOnly] private Mesh bakedMesh;

        public List<Quad> Quads => quads;
        public Mesh BakedMesh => bakedMesh;

        [field: SerializeField, HideInInspector] public bool IsBakeDirty {
            get;
#if UNITY_EDITOR
            set; //!! Editor only setter
#endif
        } = true;

        public void AddQuad(Vector3 origin) {
            quads.Add(new Quad(origin + Vector3.up * 0.01f));
        }

        public void RemoveQuad(int index) {
            quads.RemoveAt(index);
        }

        private void Reset() {
            quads = new List<Quad> {
                new Quad(Vector3.zero)
            };
        }

#if UNITY_EDITOR
        public void SetBakedMesh(Mesh mesh) {
            if (bakedMesh != null) {
                bakedMesh.Clear();
                DestroyImmediate(bakedMesh, true);
            }

            bakedMesh = mesh;
            AssetDatabase.AddObjectToAsset(bakedMesh, this);
        }
#endif

        [Serializable]
        public struct Quad {
            public Vector3[] Points;

            public Quad(Vector3 origin) {
                Points = new[] {
                    origin,
                    origin + Vector3.right,
                    origin + Vector3.right + Vector3.forward,
                    origin + Vector3.forward
                };
            }
        }
    }
}