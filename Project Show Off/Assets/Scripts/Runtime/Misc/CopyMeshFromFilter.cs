using UnityEngine;

namespace Runtime {
    public class CopyMeshFromFilter : MonoBehaviour {
        private void Awake() {
            var meshFilter = GetComponent<MeshFilter>();
            var meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.mesh;
        }
    }
}