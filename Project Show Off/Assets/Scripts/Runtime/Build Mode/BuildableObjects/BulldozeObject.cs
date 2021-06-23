using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class BulldozeObject : MonoBehaviour {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject previewObject;
        [SerializeField] private Material previewMaterial;

        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private float materialAlpha = 0.5f;

        public void HideIndicator() {
            previewMaterial.DOFade(0.0f, animationDuration)
                           .OnComplete(() => previewObject.SetActive(false));
        }

        public void ShowIndicator() {
            previewObject.SetActive(true);
            previewMaterial.DOFade(materialAlpha, animationDuration);
        }

        public void Bulldoze() {
            Destroy(root);
        }
    }
}