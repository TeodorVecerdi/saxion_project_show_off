using DG.Tweening;
using UnityEngine;

namespace Runtime {
    public class StorySlide : MonoBehaviour {
        [SerializeField] private CanvasGroup textBox;
        private CanvasGroup self;

        private void Awake() {
            self = GetComponent<CanvasGroup>();
        }

        public void Show() {
            self.DOFade(1.0f, 0.5f);
        }

        public void Hide() {
            self.DOFade(0.0f, 0.5f);
        }

        public void ShowTextBox() {
            textBox.DOFade(1.0f, 0.5f);
        }
    }
}