using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class BulldozeObject : MonoBehaviour {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject previewObject;
        [SerializeField] private Material previewMaterial;

        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private float materialAlpha = 0.5f;
        private Material material;

        private void Start() {
            material = new Material(previewMaterial);
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = material;
        }

        public void HideIndicator() {
            material.DOFade(0.0f, animationDuration)
                    .OnComplete(() => previewObject.SetActive(false));
        }

        public void ShowIndicator() {
            previewObject.SetActive(true);
            material.DOFade(materialAlpha, animationDuration);
        }

        public void Bulldoze() {
            Destroy(root);
        }
    }
}