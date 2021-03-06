using DG.Tweening;
using Runtime.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime {
    public sealed class InventoryMaterialView : MonoBehaviour {
        [SerializeField] private Image patternImage;
        [SerializeField] private Image transitionImage;
        
        public void LoadUI(TrashMaterial trashMaterial) {
            patternImage.sprite = trashMaterial.VerticalPattern;
            patternImage.type = Image.Type.Tiled;
            patternImage.pixelsPerUnitMultiplier = 1.56f; // magic value woo

            transitionImage.color = Color.clear;

            var sizeDelta = patternImage.rectTransform.sizeDelta;
            sizeDelta.y = 0.0f;
            patternImage.rectTransform.sizeDelta = sizeDelta;
        }

        public void LoadTransition(TrashMaterial otherTrashMaterial) {
            transitionImage.color = otherTrashMaterial.PatternColor;
        }

        public void UpdateSize(float screenSize) {
            var sizeDelta = patternImage.rectTransform.sizeDelta;
            sizeDelta.y = screenSize;
            patternImage.rectTransform.DOKill(true);
            patternImage.rectTransform.DOSizeDelta(sizeDelta, 0.25f);

            // debug: uncomment when fixed
            // SetTransitionEnabled(screenSize > 10.0f);
        }

        public void SetTransitionEnabled(bool isEnabled) {
            transitionImage.DOFade(isEnabled ? 1.0f : 0.0f, 0.2f);
        }
    }
}