using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Runtime.Tools {
    public class FencePlacer : MonoBehaviour {
        [SerializeField, Min(0)] private int count;

        private const float initialSpacing = 1.849f;
        private const float fenceSpacing = 1.757f;
        private static readonly Vector3 initialDelta = initialSpacing * Vector3.forward;
        private static readonly Vector3 fenceDelta = fenceSpacing * Vector3.forward;
        
        [SerializeField, HideInInspector] private GameObject fenceOpenPrefab;
        [SerializeField, HideInInspector] private GameObject fenceClosedPrefab;
        [SerializeField, HideInInspector] private GameObject fenceClosed;
        [SerializeField, HideInInspector] private List<GameObject> openFences;

        [Button]
        private void UpdateFences() {
            if (fenceOpenPrefab == null || fenceClosedPrefab == null) {
                fenceOpenPrefab = Resources.Load<GameObject>("Prefabs/Environment/Fence Open");
                fenceClosedPrefab = Resources.Load<GameObject>("Prefabs/Environment/Fence Closed");
            }
            
            openFences.ForEach(DestroyImmediate);
            openFences.Clear();
            if (count == 0) {
                DestroyImmediate(fenceClosed);
                fenceClosed = null;
            } else {
                if (fenceClosed == null) {
                    fenceClosed = Instantiate(fenceClosedPrefab, transform, false);
                }

                if (count > 1) {
                    var fenceOpen = Instantiate(fenceOpenPrefab, transform, false);
                    fenceOpen.transform.localPosition = initialDelta;
                    openFences.Add(fenceOpen);
                }
                
                for (var i = 2; i < count; i++) {
                    var fenceOpen = Instantiate(fenceOpenPrefab, transform, false);
                    fenceOpen.transform.localPosition = initialDelta + fenceDelta * (i-1);
                    openFences.Add(fenceOpen);
                }
            }
        }
    }
}