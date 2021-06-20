using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Runtime.Tools {
    public sealed class HedgePlacer : MonoBehaviour {
        [SerializeField, Min(0)] private int count;
        [SerializeField] private float spacing = 0.0f;

        private const float hedgeSpacing = 2.72f;

        [SerializeField, HideInInspector] private GameObject hedgePrefab;
        [SerializeField, HideInInspector] private List<GameObject> hedges;

        [Button]
        private void UpdateFences() {
#if UNITY_EDITOR
            if (hedgePrefab == null) {
                hedgePrefab = Resources.Load<GameObject>("Prefabs/Environment/Hedge");
            }

            hedges.ForEach(DestroyImmediate);
            hedges.Clear();
            
            for (var i = 0; i < count; i++) {
                var hedge =  PrefabUtility.InstantiatePrefab(hedgePrefab, transform) as GameObject;
                hedge!.transform.localPosition = (spacing * i + hedgeSpacing * i) * Vector3.forward;
                hedges.Add(hedge);
            }
#endif
        }
    }
}