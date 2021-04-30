using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class CrafterView : MonoBehaviour {
        [Header("References")]
        [SerializeField] private RectTransform main;
        [SerializeField] private CanvasGroup fadeMain;

        [Header("Settings")]
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float fadeDuration = 0.25f;

        private Tweener moveTweener;
        private Tweener fadeTweener;
        
        public void OpenView() {
            main.gameObject.SetActive(true);
            if(moveTweener is {active: true}) moveTweener.Kill();
            if(fadeTweener is {active: true}) moveTweener.Kill();
            moveTweener = main.DOAnchorPosY(0.77f, moveDuration);
            fadeTweener = fadeMain.DOFade(1.0f, fadeDuration);
        }

        public void CloseView() {
            if(moveTweener is {active: true}) moveTweener.Kill();
            if(fadeTweener is {active: true}) moveTweener.Kill();
            moveTweener = main.DOAnchorPosY(-0.05f, moveDuration).OnComplete(() => main.gameObject.SetActive(false));
            fadeTweener = fadeMain.DOFade(0.0f, fadeDuration);
        }
    }
}