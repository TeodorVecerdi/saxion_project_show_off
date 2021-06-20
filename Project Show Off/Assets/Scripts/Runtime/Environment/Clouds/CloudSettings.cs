using UnityEngine;

namespace Runtime {
    [ExecuteInEditMode]
    public sealed class CloudSettings : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private int cloudResolution = 20;
        [SerializeField] private float cloudHeight;
        [Space]
        [SerializeField] private bool castShadows;
        [SerializeField] private bool receiveShadows;
        [SerializeField] private bool useLightProbes;
        [Header("References")]
        [SerializeField] private Mesh cloudMesh;
        [SerializeField] private Material cloudMaterial;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Light sun;

        private float offset;
        private Matrix4x4 cloudTransform;
        
        private static readonly int cloudPositionNameID = Shader.PropertyToID("CloudPosition");
        private static readonly int cloudHeightNameID = Shader.PropertyToID("CloudHeight");

        private void Update() {
            //!! reason: repeated property access of built in component is inefficient
            var baseTransform = transform;
            
            cloudMaterial.SetFloat(cloudPositionNameID, baseTransform.position.y);
            cloudMaterial.SetFloat(cloudHeightNameID, cloudHeight);
            cloudMaterial.SetColor("MainLightColor", sun.color);
            cloudMaterial.SetVector("MainLightDirection", sun.transform.forward);
            offset = cloudHeight / cloudResolution / 2.0f;
            var initialPosition = baseTransform.position + Vector3.up * (offset * cloudResolution * 0.5f);
            for (var i = 0; i < cloudResolution; i++) {
                cloudTransform = Matrix4x4.TRS(initialPosition - Vector3.up * (offset * i), baseTransform.rotation, baseTransform.localScale);
                Graphics.DrawMesh(cloudMesh, cloudTransform, cloudMaterial, 0, mainCamera, 0, null, castShadows, receiveShadows, useLightProbes);
            }
        }
    }
}