using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class StorySlide : MonoBehaviour {
        [SerializeField] private CanvasGroup textBox;
        private CanvasGroup self;

        private void Awake() {
            self = GetComponent<CanvasGroup>();
        }

        public void Show(float fadeTime) {
            self.DOFade(1.0f, fadeTime);
        }

        public void Hide(float fadeTime) {
            self.DOFade(0.0f, fadeTime);
        }

        public void ShowTextBox(float fadeTime) {
            textBox.DOFade(1.0f, fadeTime);
        }
    }
}