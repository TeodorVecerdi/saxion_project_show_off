using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class PlaceableSelectionHandler : MonoBehaviour {
        [SerializeField] private PlaceableObject owner;
        [SerializeField] private Transform selectionPreviewTransform;
        [SerializeField] private float selectionAnimationDuration = 0.15f;
        private bool isSelected;

        public void SetSelection(bool selected) {
            if (isSelected == selected) return;
            
            isSelected = selected;
            
            selectionPreviewTransform.DOKill();
            selectionPreviewTransform.DOScaleX(isSelected ? 3.0f : 0.0f, selectionAnimationDuration);
            selectionPreviewTransform.DOScaleZ(isSelected ? 3.0f : 0.0f, selectionAnimationDuration);
        }
    }
}