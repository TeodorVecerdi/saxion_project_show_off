using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Data {
    [CreateAssetMenu(fileName = "New Build Area", menuName = "Data/Build Area", order = 0)]
    public class BuildArea : ScriptableObject {
        [SerializeField] private List<Quad> quads;
        [SerializeField, ReadOnly] private Mesh bakedMesh;

        public List<Quad> Quads => quads;
        public Mesh BakedMesh {
            get => bakedMesh;
#if UNITY_EDITOR //!! Editor only setter
            set => bakedMesh = value;
#endif
        }

        [field: SerializeField, HideInInspector] public bool IsBakeDirty {
            get;
#if UNITY_EDITOR //!! Editor only setter
            set;
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